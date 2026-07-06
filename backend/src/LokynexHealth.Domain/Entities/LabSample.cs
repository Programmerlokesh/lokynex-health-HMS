using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class LabSample : BaseEntity
{
    public Guid OrderId { get; set; }
    public string SampleBarcode { get; set; } = string.Empty;
    public Guid? CollectedBy { get; set; }
    public DateTimeOffset? CollectedAt { get; set; }
    public DateTimeOffset? ReceivedAtLab { get; set; }
    public bool Rejected { get; set; }
    public string? RejectionReason { get; set; }
}