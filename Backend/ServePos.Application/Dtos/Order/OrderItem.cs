namespace ServePOS.API.Dtos;

public class OrderItem
{
    public required int MenuItemId { get; init; }
    public string? SpecialInstructions { get; init; }
}
