namespace LokynexHealth.Application.Features.Laboratory.DTOs;

public class LabOrderDetailDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid PatientId { get; set; }
    public Guid? OrderingDoctorId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string? SchemeTag { get; set; }
    public DateTimeOffset OrderedAt { get; set; }
    public DateTimeOffset? ReleasedAt { get; set; }
    public List<LabOrderTestDto> Tests { get; set; } = new();
}