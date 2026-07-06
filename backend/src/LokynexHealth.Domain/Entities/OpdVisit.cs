using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class OpdVisit : BaseEntity
{
    public string VisitNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? TokenId { get; set; }
    public string? SchemeTag { get; set; }
    public string? ChiefComplaint { get; set; }
    public DateTimeOffset VisitStartedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? VisitEndedAt { get; set; }
    public RecordStatus Status { get; set; } = RecordStatus.Active;
    public bool SignedOff { get; set; }
    public DateTimeOffset? SignedOffAt { get; set; }
    public string? EsignQrCodeRef { get; set; }
}
