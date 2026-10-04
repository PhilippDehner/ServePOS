using ServePos.Application.Dtos.Order;
using ServePos.Application.Repositoryies;
using ServePos.Domain.Entities;

namespace ServePos.Application.Services;

public class OrderService(IOrderRepository _repo, IStaffRepository _staffRepo)
{
    public async Task Order(OrderInformation order, CancellationToken cancellationToken)
    {
        if (await _repo.ExistsByClientOrderIdAsync(order.ClientOrderId, cancellationToken))
        {
            return;
        }

        await _staffRepo.GetByIdAsync(order.StaffId, cancellationToken);
        var items = order.Items.Select(x => new Domain.Entities.OrderItem(x.MenuItemId, x.SpecialInstructions)).ToList();
        var insert = new Order(order.ClientOrderId, order.TableId, order.StaffId, items);
        await _repo.AddIfAbsentAsync(insert, cancellationToken);
    }
}
