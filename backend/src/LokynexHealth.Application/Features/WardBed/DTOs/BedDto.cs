namespace LokynexHealth.Application.Features.WardBed.DTOs;

public class BedDto
{
    public Guid Id { get; set; }
    public Guid WardId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public string? BedCategory { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? DailyRate { get; set; }
    public Guid? CurrentAdmissionId { get; set; }
}