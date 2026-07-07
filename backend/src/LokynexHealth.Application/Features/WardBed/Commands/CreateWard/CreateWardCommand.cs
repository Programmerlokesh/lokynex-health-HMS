using MediatR;

namespace LokynexHealth.Application.Features.WardBed.Commands.CreateWard;

public class CreateWardCommand : IRequest<Guid>
{
    public string WardName { get; set; } = string.Empty;
    public string? WardType { get; set; }
    public string? Floor { get; set; }
}