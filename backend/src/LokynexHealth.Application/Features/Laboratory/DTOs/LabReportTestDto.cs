namespace LokynexHealth.Application.Features.Laboratory.DTOs;

public class LabReportTestDto
{
    public string TestName { get; set; } = string.Empty;
    public string? SpecimenType { get; set; }
    public List<LabReportResultDto> Results { get; set; } = new();
}