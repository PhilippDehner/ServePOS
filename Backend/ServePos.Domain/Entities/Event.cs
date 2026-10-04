using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class Event : EntityId
{
    private Event() { }

    public Event(string name, DateTime startsAt)
    {
        Name = name;
        StartsAt = startsAt;
    }

    public string Name { get; private set; } = "";
    public DateTime StartsAt { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsCompleted { get; private set; }

    public void Activate()
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Completed events cannot be activated.");
        }

        IsActive = true;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void Complete()
    {
        IsActive = false;
        IsCompleted = true;
        UpdateTimestamp();
    }
}
