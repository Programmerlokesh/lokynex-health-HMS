namespace LokynexHealth.Application.Features.WardBed.DTOs;

public class WardDto
{
    public Guid Id { get; set; }
    public string WardName { get; set; } = string.Empty;
    public string? WardType { get; set; }
    public string? Floor { get; set; }
    public bool IsActive { get; set; }
}