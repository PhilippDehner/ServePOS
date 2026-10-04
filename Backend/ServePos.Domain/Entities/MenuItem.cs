using ServePos.Domain.Base;
using MenuItemKind = ServePos.Shared.MenuItemType;

namespace ServePos.Domain.Entities;

public class MenuItem : EntityId
{
    private MenuItem() { }

    public MenuItem(string name, string? shortName, MenuItemKind type, decimal price, short? availableQuantity, int sortIndex)
    {
        Name = name;
        ShortName = shortName;
        Type = type;
        Price = price;
        AvailableQuantity = availableQuantity;
        SortIndex = sortIndex;
        IsActive = true;
    }

    public string Name { get; private set; } = "";
    public string? ShortName { get; private set; }
    public MenuItemKind Type { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public int? AvailableQuantity { get; private set; }
    public int SortIndex { get; private set; }

    public void Update(string name, string? shortName, MenuItemKind type, decimal price, bool isActive, int? availableQuantity)
    {
        Name = name;
        ShortName = shortName;
        Type = type;
        Price = price;
        AvailableQuantity = availableQuantity;
        IsActive = isActive;

        UpdateTimestamp();
    }

    public void SetSortIndex(int index)
    {
        SortIndex = index;
    }

    public void DecreaseAvailableQuantity()
    {
        if (AvailableQuantity is null)
        {
            return;
        }

        if (AvailableQuantity <= 0)
        {
            throw new InvalidOperationException($"Menu item '{Name}' is out of stock.");
        }

        AvailableQuantity--;
        UpdateTimestamp();
    }

    public void IncreaseAvailableQuantity()
    {
        if (AvailableQuantity is null)
        {
            return;
        }

        AvailableQuantity++;
        UpdateTimestamp();
    }
}
