using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.WardBed.DTOs;

namespace LokynexHealth.Application.Features.WardBed.Queries.GetBedMap;

public class GetBedMapQueryHandler : IRequestHandler<GetBedMapQuery, List<WardBedMapDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBedMapQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WardBedMapDto>> Handle(GetBedMapQuery request, CancellationToken cancellationToken)
    {
        var wards = await _context.Wards
            .AsNoTracking()
            .Where(w => w.IsActive)
            .ToListAsync(cancellationToken);

        var beds = await _context.Beds
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var activeAdmissions = await _context.Admissions
            .AsNoTracking()
            .Where(a => a.Status == RecordStatus.Active)
            .ToListAsync(cancellationToken);

        return wards.Select(ward => new WardBedMapDto
        {
            Id = ward.Id,
            WardName = ward.WardName,
            WardType = ward.WardType,
            Floor = ward.Floor,
            Beds = beds
                .Where(b => b.WardId == ward.Id)
                .Select(b => new BedDto
                {
                    Id = b.Id,
                    WardId = b.WardId,
                    BedNumber = b.BedNumber,
                    BedCategory = b.BedCategory,
                    Status = b.Status.ToString(),
                    DailyRate = b.DailyRate,
                    CurrentAdmissionId = activeAdmissions.FirstOrDefault(a => a.BedId == b.Id)?.Id
                }).ToList()
        }).ToList();
    }
}