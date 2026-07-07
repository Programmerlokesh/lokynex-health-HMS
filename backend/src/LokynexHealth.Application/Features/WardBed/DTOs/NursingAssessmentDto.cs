namespace LokynexHealth.Application.Features.WardBed.DTOs;

public class NursingAssessmentDto
{
    public Guid Id { get; set; }
    public Guid AdmissionId { get; set; }
    public Guid? AssessedBy { get; set; }
    public string? VitalsJson { get; set; }
    public short? GcsScore { get; set; }
    public short? BradenScore { get; set; }
    public short? MorseFallScore { get; set; }
    public short? Nrs2002Score { get; set; }
    public DateTimeOffset AssessedAt { get; set; }
}