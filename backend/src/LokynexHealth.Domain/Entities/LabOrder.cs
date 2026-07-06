using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class LabOrder : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Guid? OrderingDoctorId { get; set; }
    public Guid? SourceVisitId { get; set; }
    public LabOrderStatus Status { get; set; } = LabOrderStatus.Ordered;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Routine;
    public string? SchemeTag { get; set; }
    public bool AiPanelSuggested { get; set; }
    public Guid? AiLogId { get; set; }
    public DateTimeOffset OrderedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ReleasedAt { get; set; }
}