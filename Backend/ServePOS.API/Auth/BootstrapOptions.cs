namespace ServePOS.API.Auth;

public class BootstrapOptions
{
    public const string SectionName = "BootstrapAdmin";

    public required string Username { get; init; }
    public required string Pin { get; init; }
}
