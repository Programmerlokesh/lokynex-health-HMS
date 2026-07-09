using MediatR;
using LokynexHealth.Application.Features.Icu.DTOs;

namespace LokynexHealth.Application.Features.Icu.Commands.RecordVitals;

public class RecordVitalsCommand : IRequest<IcuVitalDto>
{
    public Guid IcuAdmissionId { get; set; }
    public short? HeartRate { get; set; }
    public short? SystolicBp { get; set; }
    public short? DiastolicBp { get; set; }
    public short? MapMmhg { get; set; }
    public short? Spo2Pct { get; set; }
    public decimal? TemperatureC { get; set; }
    public decimal? UrineOutputMlPerKgHr { get; set; }
    public short? RespiratoryRate { get; set; }
}