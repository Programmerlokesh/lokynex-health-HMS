using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class BedTransfer : BaseEntity
{
    public Guid AdmissionId { get; set; }
    public Guid? FromBedId { get; set; }
    public Guid ToBedId { get; set; }
    public string? IsbarIdentify { get; set; }
    public string? IsbarSituation { get; set; }
    public string? IsbarBackground { get; set; }
    public string? IsbarAssessment { get; set; }
    public string? IsbarRecommendation { get; set; }
    public Guid? TransferredBy { get; set; }
    public DateTimeOffset TransferredAt { get; set; } = DateTimeOffset.UtcNow;
}