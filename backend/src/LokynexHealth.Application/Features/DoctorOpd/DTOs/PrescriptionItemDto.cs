namespace LokynexHealth.Application.Features.DoctorOpd.DTOs;

public class PrescriptionItemDto
{
    public Guid Id { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string? RxnormCode { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public short? DurationDays { get; set; }
    public string ScheduleFlag { get; set; } = string.Empty;
}