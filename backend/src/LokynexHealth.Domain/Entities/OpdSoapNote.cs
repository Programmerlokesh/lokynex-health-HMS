using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class OpdSoapNote : BaseEntity
{
    public Guid VisitId { get; set; }
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }
    public string SourceLanguage { get; set; } = "en";
    public bool AiDrafted { get; set; }
    public Guid? AiLogId { get; set; }
    public bool FinalizedByDoctor { get; set; }
}
