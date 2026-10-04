using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class Staff(string name) : EntityId
{
    public string Name { get; private set; } = name;
    public DateTime LastOperation { get; private set; } = DateTime.UtcNow;

    public void Update(string name)
    {
        Name = name;
    }

    public void UpdateLastOperation()
    {
        LastOperation = DateTime.UtcNow;
    }
}
