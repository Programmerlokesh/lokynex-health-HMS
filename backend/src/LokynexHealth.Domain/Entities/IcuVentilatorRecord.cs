using LokynexHealth.Domain.Common;

namespace LokynexHealth.Domain.Entities;

public class IcuVentilatorRecord : BaseEntity
{
    public Guid IcuAdmissionId { get; set; }
    public string? Mode { get; set; }
    public short? Fio2Pct { get; set; }
    public decimal? PeepCmh2o { get; set; }
    public short? TidalVolumeMl { get; set; }
    public DateTimeOffset RecordedAt { get; set; } = DateTimeOffset.UtcNow;
}