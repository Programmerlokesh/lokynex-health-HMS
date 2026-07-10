namespace LokynexHealth.Application.Features.Laboratory.DTOs;

public class LabReportDto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset OrderedAt { get; set; }
    public DateTimeOffset? ReleasedAt { get; set; }

    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientUhid { get; set; } = string.Empty;
    public int? PatientAge { get; set; }
    public string PatientGender { get; set; } = string.Empty;

    public Guid? OrderingDoctorId { get; set; }
    public string? OrderingDoctorName { get; set; }
    public string? OrderingDoctorRegistrationNo { get; set; }

    public List<LabReportTestDto> Tests { get; set; } = new();
}