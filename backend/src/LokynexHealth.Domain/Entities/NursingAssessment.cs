using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class NursingAssessment : BaseEntity
{
    public Guid AdmissionId { get; set; }
    public Guid? AssessedBy { get; set; }
    public string? VitalsJson { get; set; }
    public short? GcsScore { get; set; }
    public short? BradenScore { get; set; }
    public short? MorseFallScore { get; set; }
    public short? Nrs2002Score { get; set; }
    public decimal? AiLosPredictionDays { get; set; }
    public decimal? AiReadmissionRisk { get; set; }
    public Guid? AiLogId { get; set; }
    public DateTimeOffset AssessedAt { get; set; } = DateTimeOffset.UtcNow;
}