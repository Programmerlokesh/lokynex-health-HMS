using MediatR;

namespace LokynexHealth.Application.Features.Laboratory.Commands.ReleaseLabOrder;

public class ReleaseLabOrderCommand : IRequest<Guid>
{
    public Guid OrderId { get; set; }
}