using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.WardBed.Commands.CompleteHousekeepingTask;

public class CompleteHousekeepingTaskCommandHandler : IRequestHandler<CompleteHousekeepingTaskCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CompleteHousekeepingTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CompleteHousekeepingTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.HousekeepingTasks
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task is null)
        {
            throw new KeyNotFoundException($"Housekeeping task with Id '{request.TaskId}' was not found.");
        }

        if (task.Status == "done")
        {
            throw new InvalidOperationException("Housekeeping task is already completed.");
        }

        task.Status = "done";
        task.CompletedAt = DateTimeOffset.UtcNow;

        var bed = await _context.Beds
            .FirstOrDefaultAsync(b => b.Id == task.BedId, cancellationToken);

        if (bed is not null)
        {
            bed.Status = BedStatus.Available;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}