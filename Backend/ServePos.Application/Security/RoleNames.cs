namespace ServePos.Application.Security;

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Cashier = "Cashier";
    public const string Kitchen = "Kitchen";
    public const string Drinks = "Drinks";

    public static readonly string[] All = [Admin, Cashier, Kitchen, Drinks];
}
