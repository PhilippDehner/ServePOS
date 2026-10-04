using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class OrderItem : EntityId
{
    private OrderItem() { }
    public OrderItem(int menuItemId, string? specialInstructions = null)
    {
        MenuItemId = menuItemId;
        SpecialInstructions = specialInstructions;
    }

    public int OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public int MenuItemId { get; private set; }
    public MenuItem MenuItem { get; private set; } = null!;

    public string? SpecialInstructions { get; private set; }
    public decimal? UnitPrice { get; private set; }

    public void SetUnitPrice(decimal unitPrice)
    {
        UnitPrice = unitPrice;
        UpdateTimestamp();
    }
}
