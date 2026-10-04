using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class Order : EntityId
{
    private Order() { }
    public Order(Guid clientOrderId, string tableId, int staffId, List<OrderItem> items)
    {
        ClientOrderId = clientOrderId;
        TableId = tableId;
        EnteredById = staffId;
        Items = items;
    }

    public Guid? ClientOrderId { get; private set; }
    public string TableId { get; set; } = null!;
    public bool IsPrinted { get; private set; }
    public bool IsPaid { get; private set; }
    public CashPayment? CashPayment { get; private set; }

    public int EnteredById { get; private set; }
    public Staff EnteredBy { get; private set; } = null!;
    public int? EventId { get; private set; }
    public Event? Event { get; private set; }

    public List<OrderItem> Items { get; private set; } = [];

    public void AssignEvent(int eventId)
    {
        EventId = eventId;
    }

    public void MarkPaid(CashPayment cashPayment)
    {
        if (IsPaid)
        {
            throw new InvalidOperationException("The order has already been paid.");
        }

        CashPayment = cashPayment;
        IsPaid = true;
        UpdateTimestamp();
    }
}
