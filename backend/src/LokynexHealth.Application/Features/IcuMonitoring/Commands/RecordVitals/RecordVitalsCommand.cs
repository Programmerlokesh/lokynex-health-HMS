using MediatR;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.RecordVitals;

public class RecordVitalsCommand : IRequest<Guid>
{
    public Guid IcuAdmissionId { get; set; }
    public short? HeartRate { get; set; }
    public short? SystolicBp { get; set; }
    public short? DiastolicBp { get; set; }
    public short? MapMmHg { get; set; }
    public short? SpO2Pct { get; set; }
    public decimal? TemperatureC { get; set; }
    public decimal? UrineOutputMlPerKgHr { get; set; }
    public short? RespiratoryRate { get; set; }
}