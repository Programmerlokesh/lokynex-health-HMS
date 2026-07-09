using MediatR;

namespace LokynexHealth.Application.Features.Icu.Commands.RecordVentilatorSettings;

public class RecordVentilatorSettingsCommand : IRequest<Guid>
{
    public Guid IcuAdmissionId { get; set; }
    public string? Mode { get; set; }
    public short? Fio2Pct { get; set; }
    public decimal? PeepCmh2o { get; set; }
    public short? TidalVolumeMl { get; set; }
}