using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class Table : EntityId
{
    private Table() { }

    public Table(string name, int eventId)
    {
        Name = name;
        EventId = eventId;
    }

    public string Name { get; private set; } = "";
    public int EventId { get; private set; }
    public Event Event { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdateTimestamp();
    }
}
