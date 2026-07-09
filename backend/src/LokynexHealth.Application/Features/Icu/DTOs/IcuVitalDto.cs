namespace LokynexHealth.Application.Features.Icu.DTOs;

public class IcuVitalDto
{
    public Guid Id { get; set; }
    public Guid IcuAdmissionId { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
    public short? HeartRate { get; set; }
    public short? SystolicBp { get; set; }
    public short? DiastolicBp { get; set; }
    public short? MapMmhg { get; set; }
    public short? Spo2Pct { get; set; }
    public decimal? TemperatureC { get; set; }
    public decimal? UrineOutputMlPerKgHr { get; set; }
    public short? RespiratoryRate { get; set; }
    public bool IsThresholdBreach { get; set; }
    public List<string> BreachParameters { get; set; } = new();
}