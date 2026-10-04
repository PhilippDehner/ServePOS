using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class Role : EntityId
{
    private Role() { }

    public Role(string name)
    {
        Name = name;
    }

    public string Name { get; private set; } = "";
}
