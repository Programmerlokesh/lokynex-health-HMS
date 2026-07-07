namespace LokynexHealth.Application.Features.Laboratory.DTOs;

public class LabOrderTestDto
{
    public Guid Id { get; set; }
    public Guid TestId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public decimal PriceApplied { get; set; }
    public List<LabResultDto> Results { get; set; } = new();
}