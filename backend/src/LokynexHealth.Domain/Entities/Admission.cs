using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class Admission : BaseEntity
{
    public string AdmissionNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Guid? AdmittingDoctorId { get; set; }
    public Guid BedId { get; set; }
    public string? PmjayPackageCode { get; set; }
    public DateTimeOffset AdmittedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DischargedAt { get; set; }
    public RecordStatus Status { get; set; } = RecordStatus.Active;
}