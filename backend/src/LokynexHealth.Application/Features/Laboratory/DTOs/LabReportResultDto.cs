namespace LokynexHealth.Application.Features.Laboratory.DTOs;

public class LabReportResultDto
{
    public string ParameterName { get; set; } = string.Empty;
    public string? ResultValue { get; set; }
    public string? Unit { get; set; }
    public string? ReferenceRange { get; set; }
    public bool IsAbnormal { get; set; }
    public bool IsCritical { get; set; }
    public DateTimeOffset? ValidatedAt { get; set; }
}