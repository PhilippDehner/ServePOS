namespace ServePOS.API.Controllers;

public class EventUpsert
{
    public required string Name { get; init; }
    public required DateTime StartsAt { get; init; }
}

public class EventInfo
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required DateTime StartsAt { get; init; }
    public required bool IsActive { get; init; }
    public required bool IsCompleted { get; init; }
}

public class UserUpsert
{
    public required string Username { get; init; }
    public required string Pin { get; init; }
    public required int RoleId { get; init; }
}

public class UserInfo
{
    public required int Id { get; init; }
    public required string Username { get; init; }
    public required int RoleId { get; init; }
    public required string RoleName { get; init; }
    public required bool IsActive { get; init; }
}

public class NamedResourceUpsert
{
    public required string Name { get; init; }
}

public class ServingStationUpsert
{
    public required string Name { get; init; }
    public required int EventId { get; init; }
}

public class MenuItemTypeUpsert
{
    public required string Name { get; init; }
    public required int ServingStationId { get; init; }
}
