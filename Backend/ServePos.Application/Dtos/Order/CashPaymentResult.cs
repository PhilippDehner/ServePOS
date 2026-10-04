namespace ServePos.Application.Dtos.Order;

public class CashPaymentResult
{
    public required int OrderId { get; init; }
    public required decimal TotalAmount { get; init; }
    public required decimal ReceivedAmount { get; init; }
    public required decimal ChangeAmount { get; init; }
}
