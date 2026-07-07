using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.WardBed.Commands.CreateBed;

public class CreateBedCommandHandler : IRequestHandler<CreateBedCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateBedCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateBedCommand request, CancellationToken cancellationToken)
    {
        var wardExists = await _context.Wards
            .AnyAsync(w => w.Id == request.WardId && w.IsActive, cancellationToken);

        if (!wardExists)
        {
            throw new KeyNotFoundException($"Active ward with Id '{request.WardId}' was not found.");
        }

        var duplicateExists = await _context.Beds
            .AnyAsync(b => b.WardId == request.WardId && b.BedNumber == request.BedNumber, cancellationToken);

        if (duplicateExists)
        {
            throw new InvalidOperationException($"Bed number '{request.BedNumber}' already exists in this ward.");
        }

        var bed = new Bed
        {
            WardId = request.WardId,
            BedNumber = request.BedNumber,
            BedCategory = request.BedCategory,
            DailyRate = request.DailyRate,
            Status = BedStatus.Available
        };

        _context.Beds.Add(bed);
        await _context.SaveChangesAsync(cancellationToken);

        return bed.Id;
    }
}