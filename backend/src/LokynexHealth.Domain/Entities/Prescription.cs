using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class Prescription : BaseEntity
{
    public string PrescriptionNumber { get; set; } = string.Empty;
    public Guid VisitId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTimeOffset IssuedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool PushedToAbha { get; set; }
    public bool PushedToPharmacyQueue { get; set; }
}
