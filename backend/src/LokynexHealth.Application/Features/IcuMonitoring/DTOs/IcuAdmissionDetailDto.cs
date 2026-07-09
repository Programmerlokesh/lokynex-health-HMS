namespace LokynexHealth.Application.Features.IcuMonitoring.DTOs;

public class IcuAdmissionDetailDto
{
    public Guid Id { get; set; }
    public Guid AdmissionId { get; set; }
    public string IcuUnitType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset AdmittedAt { get; set; }
    public DateTimeOffset? DischargedAt { get; set; }
    public short? ApacheIiScore { get; set; }
    public short? SofaScore { get; set; }
    public List<IcuVitalDto> LatestVitals { get; set; } = new();
}