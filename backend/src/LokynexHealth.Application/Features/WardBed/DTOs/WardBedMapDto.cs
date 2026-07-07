namespace LokynexHealth.Application.Features.WardBed.DTOs;

public class WardBedMapDto
{
    public Guid Id { get; set; }
    public string WardName { get; set; } = string.Empty;
    public string? WardType { get; set; }
    public string? Floor { get; set; }
    public List<BedDto> Beds { get; set; } = new();
}