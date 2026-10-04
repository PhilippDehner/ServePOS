namespace ServePos.Application.Dtos.Order;

public class CashPaymentInformation
{
    public required decimal ReceivedAmount { get; init; }
    public required int StaffId { get; init; }
}
