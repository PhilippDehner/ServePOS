using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class MenuItemType : EntityId
{
    private MenuItemType() { }

    public MenuItemType(string name, int servingStationId)
    {
        Name = name;
        ServingStationId = servingStationId;
    }

    public string Name { get; private set; } = "";
    public int ServingStationId { get; private set; }
    public ServingStation ServingStation { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdateTimestamp();
    }
}
