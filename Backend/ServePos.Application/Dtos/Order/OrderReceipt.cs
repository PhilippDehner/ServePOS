namespace ServePos.Application.Dtos.Order;

public class OrderReceipt
{
    public required int OrderId { get; init; }
    public required bool IsPaid { get; init; }
}
