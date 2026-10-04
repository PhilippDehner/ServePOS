using ServePos.Domain.Base;

namespace ServePos.Domain.Entities;

public class AuditEvent : EntityId
{
    private AuditEvent() { }

    public AuditEvent(string action, string entityType, int entityId, int staffId, string deviceId, string details)
    {
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        StaffId = staffId;
        DeviceId = deviceId;
        Details = details;
    }

    public string Action { get; private set; } = "";
    public string EntityType { get; private set; } = "";
    public int EntityId { get; private set; }
    public int StaffId { get; private set; }
    public Staff Staff { get; private set; } = null!;
    public string DeviceId { get; private set; } = "";
    public string Details { get; private set; } = "";
}
