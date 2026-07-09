namespace LokynexHealth.Application.Features.IcuMonitoring.DTOs;

public class IcuVentilatorRecordDto
{
    public Guid Id { get; set; }
    public Guid IcuAdmissionId { get; set; }
    public string? Mode { get; set; }
    public short? Fio2Pct { get; set; }
    public decimal? PeepCmh2o { get; set; }
    public short? TidalVolumeMl { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
}
