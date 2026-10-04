using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class CashPayment : EntityId
{
    private CashPayment() { }

    public CashPayment(decimal receivedAmount, decimal totalAmount, int receivedById)
    {
        if (receivedAmount < totalAmount)
        {
            throw new InvalidOperationException("The received amount is less than the order total.");
        }

        ReceivedAmount = receivedAmount;
        TotalAmount = totalAmount;
        ChangeAmount = receivedAmount - totalAmount;
        ReceivedById = receivedById;
    }

    public int OrderId { get; private set; }
    public Order Order { get; private set; } = null!;
    public decimal ReceivedAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal ChangeAmount { get; private set; }
    public int ReceivedById { get; private set; }
    public Staff ReceivedBy { get; private set; } = null!;
}
