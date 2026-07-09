namespace LokynexHealth.Application.Features.Icu.DTOs;

public class IcuAdmissionDto
{
    public Guid Id { get; set; }
    public Guid AdmissionId { get; set; }
    public string IcuUnitType { get; set; } = string.Empty;
    public DateTimeOffset AdmittedAt { get; set; }
    public DateTimeOffset? DischargedAt { get; set; }
    public short? ApacheIiScore { get; set; }
    public short? SofaScore { get; set; }
    public string Status { get; set; } = string.Empty;
}