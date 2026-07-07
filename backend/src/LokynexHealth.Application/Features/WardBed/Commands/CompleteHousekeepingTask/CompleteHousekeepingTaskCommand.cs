using MediatR;

namespace LokynexHealth.Application.Features.WardBed.Commands.CompleteHousekeepingTask;

public class CompleteHousekeepingTaskCommand : IRequest<Guid>
{
    public Guid TaskId { get; set; }
}