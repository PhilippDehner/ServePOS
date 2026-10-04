using ServePos.Application.Dtos.Order;
using ServePos.Domain.Entities;

namespace ServePos.Application.Repositoryies;

public interface IOrderRepository
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<bool> AddIfAbsentAsync(Order order, CancellationToken cancellationToken);
    Task<bool> ExistsByClientOrderIdAsync(Guid clientOrderId, CancellationToken cancellationToken);
    Task<Order?> GetByClientOrderIdAsync(Guid clientOrderId, CancellationToken cancellationToken);
    Task<CashPaymentResult> PayCashAsync(int orderId, decimal receivedAmount, int staffId, CancellationToken cancellationToken);
}
