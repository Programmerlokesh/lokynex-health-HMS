using MediatR;

namespace LokynexHealth.Application.Features.WardBed.Commands.CreateBed;

public class CreateBedCommand : IRequest<Guid>
{
    public Guid WardId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public string? BedCategory { get; set; }
    public decimal? DailyRate { get; set; }
}