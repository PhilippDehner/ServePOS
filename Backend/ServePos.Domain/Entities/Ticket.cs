using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class Ticket : EntityId
{
    private Ticket() { }

    public Ticket(int orderId, TicketStation station, IEnumerable<int> orderItemIds)
    {
        OrderId = orderId;
        Station = station;
        Items = orderItemIds.Select(x => new TicketItem(x)).ToList();
    }

    public int OrderId { get; private set; }
    public Order Order { get; private set; } = null!;
    public TicketStation Station { get; private set; }
    public int PrintCount { get; private set; } = 1;
    public DateTime LastPrintedAt { get; private set; } = DateTime.UtcNow;
    public List<TicketItem> Items { get; private set; } = [];

    public void MarkReprinted()
    {
        PrintCount++;
        LastPrintedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }
}
