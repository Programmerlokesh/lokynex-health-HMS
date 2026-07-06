using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class LabResult : BaseEntity
{
    public Guid OrderTestId { get; set; }
    public Guid? SampleId { get; set; }
    public string ParameterName { get; set; } = string.Empty;
    public string? ResultValue { get; set; }
    public string? Unit { get; set; }
    public string? ReferenceRange { get; set; }
    public bool IsCritical { get; set; }
    public bool IsAbnormal { get; set; }
    public Guid? EnteredBy { get; set; }
    public Guid? ValidatedBy { get; set; }
    public DateTimeOffset? ValidatedAt { get; set; }
    public string Source { get; set; } = "manual";
}