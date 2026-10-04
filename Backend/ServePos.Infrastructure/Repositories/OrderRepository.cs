using Microsoft.EntityFrameworkCore;
using Npgsql;
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
}
