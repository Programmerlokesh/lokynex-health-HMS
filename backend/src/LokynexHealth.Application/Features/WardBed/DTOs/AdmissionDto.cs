namespace LokynexHealth.Application.Features.WardBed.DTOs;

public class AdmissionDto
{
    public Guid Id { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Guid? AdmittingDoctorId { get; set; }
    public Guid BedId { get; set; }
    public string? PmjayPackageCode { get; set; }
    public DateTimeOffset AdmittedAt { get; set; }
    public DateTimeOffset? DischargedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}