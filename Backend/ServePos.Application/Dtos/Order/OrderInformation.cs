using ServePOS.API.Dtos;

namespace ServePos.Application.Dtos.Order;

public class OrderInformation
{
    public required Guid ClientOrderId { get; init; }
    public required int StaffId { get; init; }
    public required string TableId { get; init; }
    public required List<OrderItem> Items { get; init; }
}
