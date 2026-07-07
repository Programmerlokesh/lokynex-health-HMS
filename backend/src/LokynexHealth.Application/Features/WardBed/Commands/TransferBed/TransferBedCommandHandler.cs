using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.WardBed.Commands.TransferBed;

public class TransferBedCommandHandler : IRequestHandler<TransferBedCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public TransferBedCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(TransferBedCommand request, CancellationToken cancellationToken)
    {
        var admission = await _context.Admissions
            .FirstOrDefaultAsync(a => a.Id == request.AdmissionId, cancellationToken);

        if (admission is null)
        {
            throw new KeyNotFoundException($"Admission with Id '{request.AdmissionId}' was not found.");
        }

        if (admission.Status != RecordStatus.Active)
        {
            throw new InvalidOperationException("Cannot transfer a bed for an admission that is not active.");
        }

        var toBed = await _context.Beds
            .FirstOrDefaultAsync(b => b.Id == request.ToBedId, cancellationToken);

        if (toBed is null)
        {
            throw new KeyNotFoundException($"Bed with Id '{request.ToBedId}' was not found.");
        }

        if (toBed.Status != BedStatus.Available)
        {
            throw new InvalidOperationException($"Bed '{toBed.BedNumber}' is not available (current status: {toBed.Status}).");
        }

        var fromBed = await _context.Beds
            .FirstOrDefaultAsync(b => b.Id == admission.BedId, cancellationToken);

        var transfer = new BedTransfer
        {
            AdmissionId = admission.Id,
            FromBedId = admission.BedId,
            ToBedId = request.ToBedId,
            IsbarIdentify = request.IsbarIdentify,
            IsbarSituation = request.IsbarSituation,
            IsbarBackground = request.IsbarBackground,
            IsbarAssessment = request.IsbarAssessment,
            IsbarRecommendation = request.IsbarRecommendation,
            TransferredBy = request.TransferredBy
        };

        // Free up the old bed -> goes to cleaning, housekeeping task auto-created
        if (fromBed is not null)
        {
            fromBed.Status = BedStatus.Cleaning;
            _context.HousekeepingTasks.Add(new HousekeepingTask
            {
                BedId = fromBed.Id,
                TaskType = "terminal_cleaning",
                Status = "pending"
            });
        }

        toBed.Status = BedStatus.Occupied;
        admission.BedId = request.ToBedId;

        _context.BedTransfers.Add(transfer);
        await _context.SaveChangesAsync(cancellationToken);

        return transfer.Id;
    }
}