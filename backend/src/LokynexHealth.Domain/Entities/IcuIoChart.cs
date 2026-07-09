using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class IcuIoChart : BaseEntity
{
    public Guid IcuAdmissionId { get; set; }
    public DateOnly ChartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public decimal IntakeMl { get; set; }
    public decimal OutputMl { get; set; }
    public Guid? RecordedBy { get; set; }
    public DateTimeOffset RecordedAt { get; set; } = DateTimeOffset.UtcNow;
}