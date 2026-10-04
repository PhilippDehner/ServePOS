using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class User : EntityId
{
    private User() { }

    public User(string username, string pinHash, int roleId)
    {
        Username = username;
        PinHash = pinHash;
        RoleId = roleId;
    }

    public string Username { get; private set; } = "";
    public string PinHash { get; private set; } = "";
    public int RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    public void ChangePin(string pinHash)
    {
        PinHash = pinHash;
        UpdateTimestamp();
    }

    public void ChangeRole(int roleId)
    {
        RoleId = roleId;
        UpdateTimestamp();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdateTimestamp();
    }
}
