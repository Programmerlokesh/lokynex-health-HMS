using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.WardBed.Commands.DischargePatient;

public class DischargePatientCommandHandler : IRequestHandler<DischargePatientCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public DischargePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(DischargePatientCommand request, CancellationToken cancellationToken)
    {
        var admission = await _context.Admissions
            .FirstOrDefaultAsync(a => a.Id == request.AdmissionId, cancellationToken);

        if (admission is null)
        {
            throw new KeyNotFoundException($"Admission with Id '{request.AdmissionId}' was not found.");
        }

        if (admission.Status != RecordStatus.Active)
        {
            throw new InvalidOperationException("Admission is already discharged.");
        }

        var bed = await _context.Beds
            .FirstOrDefaultAsync(b => b.Id == admission.BedId, cancellationToken);

        admission.DischargedAt = DateTimeOffset.UtcNow;
        admission.Status = RecordStatus.Completed;

        if (bed is not null)
        {
            bed.Status = BedStatus.Cleaning;
            _context.HousekeepingTasks.Add(new HousekeepingTask
            {
                BedId = bed.Id,
                TaskType = "terminal_cleaning",
                Status = "pending"
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return admission.Id;
    }
}