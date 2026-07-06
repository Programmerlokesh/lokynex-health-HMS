using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class OpdDiagnosis : BaseEntity
{
    public Guid VisitId { get; set; }
    public string Icd10Code { get; set; } = string.Empty;
    public string? Icd10Description { get; set; }
    public bool IsPrimary { get; set; }
    public bool AiSuggested { get; set; }
    public Guid? AiLogId { get; set; }
}
