using ServePos.Application.Dtos.Order;
using ServePos.Application.Repositoryies;
using ServePos.Domain.Entities;

namespace ServePos.Application.Services;

public class OrderService(IOrderRepository _repo, IStaffRepository _staffRepo)
{
    public async Task<OrderReceipt> Order(OrderInformation order, CancellationToken cancellationToken)
    {
        if (await _repo.ExistsByClientOrderIdAsync(order.ClientOrderId, cancellationToken))
        {
            var existingOrder = await _repo.GetByClientOrderIdAsync(order.ClientOrderId, cancellationToken)
                ?? throw new InvalidOperationException("The existing order could not be loaded.");
            return new OrderReceipt { OrderId = existingOrder.Id, IsPaid = existingOrder.IsPaid };
        }

        await _staffRepo.GetByIdAsync(order.StaffId, cancellationToken);
        var items = order.Items.Select(x => new Domain.Entities.OrderItem(x.MenuItemId, x.SpecialInstructions)).ToList();
        var insert = new Order(order.ClientOrderId, order.TableId, order.StaffId, items);
        var added = await _repo.AddIfAbsentAsync(insert, cancellationToken);
        if (!added)
        {
            var existingOrder = await _repo.GetByClientOrderIdAsync(order.ClientOrderId, cancellationToken)
                ?? throw new InvalidOperationException("The concurrent order could not be loaded.");
            return new OrderReceipt { OrderId = existingOrder.Id, IsPaid = existingOrder.IsPaid };
        }

        return new OrderReceipt { OrderId = insert.Id, IsPaid = false };
    }

    public async Task<CashPaymentResult> PayCash(
        int orderId,
        CashPaymentInformation payment,
        CancellationToken cancellationToken)
    {
        await _staffRepo.GetByIdAsync(payment.StaffId, cancellationToken);
        return await _repo.PayCashAsync(orderId, payment.ReceivedAmount, payment.StaffId, cancellationToken);
    }
}
