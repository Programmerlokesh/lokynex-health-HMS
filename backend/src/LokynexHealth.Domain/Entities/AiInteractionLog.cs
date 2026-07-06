using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Enums;

namespace LokynexHealth.Domain.Entities;

public class AiInteractionLog : BaseEntity
{
    public string ModuleName { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string? SourceRecordTable { get; set; }
    public Guid? SourceRecordId { get; set; }
    public string? InputPayload { get; set; }
    public string? AiSuggestion { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public string? ModelName { get; set; }
    public string? ModelVersion { get; set; }
    public AiReviewStatus ReviewStatus { get; set; } = AiReviewStatus.AiSuggested;
    public Guid? ReviewedByStaffId { get; set; }
    public string? OverrideReason { get; set; }
    public bool PhiSentExternally { get; set; }
}
