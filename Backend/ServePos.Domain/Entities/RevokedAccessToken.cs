namespace ServePos.Domain.Entities;

public class RevokedAccessToken
{
    public RevokedAccessToken(string tokenId, DateTime expiresAt)
    {
        TokenId = tokenId;
        ExpiresAt = expiresAt;
    }

    public string TokenId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
}
