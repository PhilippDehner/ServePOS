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

    public int EnteredById { get; private set; }
    public Staff EnteredBy { get; private set; } = null!;

    public List<OrderItem> Items { get; private set; } = [];
}
