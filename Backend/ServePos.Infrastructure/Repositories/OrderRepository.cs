using Microsoft.EntityFrameworkCore;
using Npgsql;
using ServePos.Application.Dtos.Order;
using ServePos.Application.Repositoryies;
using ServePos.Domain.Entities;

namespace ServePos.Infrastructure.Repositories;

public class OrderRepository(PosDbContext db) : Repository(db), IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        Context.Orders.Add(order);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByClientOrderIdAsync(Guid clientOrderId, CancellationToken cancellationToken)
    {
        return Context.Orders.AnyAsync(
            order => order.ClientOrderId == clientOrderId,
            cancellationToken);
    }

    public async Task<bool> AddIfAbsentAsync(Order order, CancellationToken cancellationToken)
    {
        Context.Orders.Add(order);

        try
        {
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
            })
        {
            Context.ChangeTracker.Clear();
            return false;
        }
    }

    public Task<Order?> GetByClientOrderIdAsync(Guid clientOrderId, CancellationToken cancellationToken) =>
        Context.Orders.SingleOrDefaultAsync(x => x.ClientOrderId == clientOrderId, cancellationToken);

    public async Task<CashPaymentResult> PayCashAsync(
        int orderId,
        decimal receivedAmount,
        int staffId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable,
            cancellationToken);

        var order = await Context.Orders
            .Include(x => x.Items)
            .ThenInclude(x => x.MenuItem)
            .SingleOrDefaultAsync(x => x.Id == orderId, cancellationToken)
            ?? throw new InvalidOperationException("The order does not exist.");

        if (order.IsPaid)
        {
            throw new InvalidOperationException("The order has already been paid.");
        }

        var totalAmount = order.Items.Sum(x => x.MenuItem.Price);
        var payment = new CashPayment(receivedAmount, totalAmount, staffId);

        foreach (var item in order.Items)
        {
            item.SetUnitPrice(item.MenuItem.Price);
            item.MenuItem.DecreaseAvailableQuantity();
        }

        order.MarkPaid(payment);
        var kitchenItems = order.Items
            .Where(x => x.MenuItem.Type is ServePos.Shared.MenuItemType.Food or ServePos.Shared.MenuItemType.Dessert)
            .Select(x => x.Id);
        foreach (var itemId in kitchenItems)
        {
            Context.Tickets.Add(new Ticket(order.Id, TicketStation.Kitchen, [itemId]));
        }

        var drinkItems = order.Items
            .Where(x => x.MenuItem.Type is ServePos.Shared.MenuItemType.Drink or ServePos.Shared.MenuItemType.Special)
            .Select(x => x.Id)
            .ToList();
        if (drinkItems.Count > 0)
        {
            Context.Tickets.Add(new Ticket(order.Id, TicketStation.Drinks, drinkItems));
        }

        await Context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new CashPaymentResult
        {
            OrderId = order.Id,
            TotalAmount = payment.TotalAmount,
            ReceivedAmount = payment.ReceivedAmount,
            ChangeAmount = payment.ChangeAmount,
        };
    }
}
