namespace LokynexHealth.Application.Features.IcuMonitoring.DTOs;

public class IcuAdmissionSummaryDto
{
    public Guid Id { get; set; }
    public Guid AdmissionId { get; set; }
    public string IcuUnitType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset AdmittedAt { get; set; }
    public short? ApacheIiScore { get; set; }
    public short? SofaScore { get; set; }
}