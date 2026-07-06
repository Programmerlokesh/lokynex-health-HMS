using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class LabCriticalAlert : BaseEntity
{
    public Guid ResultId { get; set; }
    public Guid? NotifiedDoctorId { get; set; }
    public string NotifiedVia { get; set; } = "sms";
    public bool AiPreAlert { get; set; }
    public Guid? AiLogId { get; set; }
    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? AcknowledgedAt { get; set; }
}