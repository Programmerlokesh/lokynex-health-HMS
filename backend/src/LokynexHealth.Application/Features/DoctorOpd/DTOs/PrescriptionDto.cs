namespace LokynexHealth.Application.Features.DoctorOpd.DTOs;

public class PrescriptionDto
{
    public Guid Id { get; set; }
    public string PrescriptionNumber { get; set; } = string.Empty;
    public Guid VisitId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public bool PushedToAbha { get; set; }
    public bool PushedToPharmacyQueue { get; set; }
    public List<PrescriptionItemDto> Items { get; set; } = new();
}