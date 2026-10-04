using ServePos.Shared;

namespace ServePos.Application.Dtos.Menu;

public class MenuItemUpdateInformation
{
    public required string Name { get; init; }
    public required string? ShortName { get; init; }
    public required decimal Price { get; init; }
    public required MenuItemType Type { get; init; }
    public short? AvailableQuantity { get; init; }
    public bool IsActive { get; internal set; }
}
