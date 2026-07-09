using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class IcuAiScore : BaseEntity
{
    public Guid IcuAdmissionId { get; set; }
    public string ScoreType { get; set; } = string.Empty;
    public decimal? ScoreValue { get; set; }
    public short? PredictedDeteriorationWindowHours { get; set; }
    public Guid? AiLogId { get; set; }
    public DateTimeOffset ComputedAt { get; set; } = DateTimeOffset.UtcNow;
}