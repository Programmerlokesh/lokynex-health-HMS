namespace LokynexHealth.Application.Features.Laboratory.DTOs;

public class LabTestDto
{
    public Guid Id { get; set; }
    public string TestCode { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public string? SpecimenType { get; set; }
    public decimal StandardPrice { get; set; }
    public decimal? TatHoursStd { get; set; }
    public bool IsActive { get; set; }
}