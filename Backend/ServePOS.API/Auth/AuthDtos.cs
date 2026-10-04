namespace ServePOS.API.Auth;

public class LoginRequest
{
    public required string Username { get; init; }
    public required string Pin { get; init; }
}

public class LoginResponse
{
    public required string AccessToken { get; init; }
    public required DateTime ExpiresAt { get; init; }
}
