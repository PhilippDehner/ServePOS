using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class OrderCancellation : EntityId
{
    private OrderCancellation() { }

    public OrderCancellation(int orderItemId, string reason, decimal refundAmount, int performedById, string deviceId)
    {
        OrderItemId = orderItemId;
        Reason = reason;
        RefundAmount = refundAmount;
        PerformedById = performedById;
        DeviceId = deviceId;
    }

    public int OrderItemId { get; private set; }
    public OrderItem OrderItem { get; private set; } = null!;
    public string Reason { get; private set; } = "";
    public decimal RefundAmount { get; private set; }
    public int PerformedById { get; private set; }
    public Staff PerformedBy { get; private set; } = null!;
    public string DeviceId { get; private set; } = "";
}
