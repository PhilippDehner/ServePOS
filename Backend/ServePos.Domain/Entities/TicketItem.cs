using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class TicketItem : EntityId
{
    private TicketItem() { }

    public TicketItem(int orderItemId)
    {
        OrderItemId = orderItemId;
    }

    public int TicketId { get; private set; }
    public Ticket Ticket { get; private set; } = null!;
    public int OrderItemId { get; private set; }
    public OrderItem OrderItem { get; private set; } = null!;
}
