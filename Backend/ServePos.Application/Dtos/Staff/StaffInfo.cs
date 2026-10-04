namespace ServePos.Application.Dtos.Staff;

public class StaffInfo
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required DateTime LastOperation { get; init; } 
}
