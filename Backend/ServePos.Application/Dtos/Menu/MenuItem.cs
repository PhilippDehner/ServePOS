using ServePos.Shared;

namespace ServePos.Application.Dtos.Menu;

public class MenuItem
{
    public required int Id { get; init; }
    public required MenuItemType Type { get; init; }
    public required string Name { get; init; }
    public required string? ShortName { get; init; }
    public required decimal Price { get; init; }
    public required int? AvailableQuantity { get; init; }
    public required int SoldQuantity { get; init; }
    public required int SortIndex { get; init; }
}
