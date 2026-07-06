using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Pharmacy.DTOs;

namespace LokynexHealth.Application.Features.Pharmacy.Queries.GetAllDrugs;

public class GetAllDrugsQueryHandler : IRequestHandler<GetAllDrugsQuery, List<DrugDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllDrugsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DrugDto>> Handle(GetAllDrugsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PharmacyDrugCatalog.AsNoTracking().AsQueryable();

        if (!request.IncludeInactive)
        {
            query = query.Where(d => d.IsActive);
        }

        return await query
            .Select(d => new DrugDto
            {
                Id = d.Id,
                DrugName = d.DrugName,
                GenericName = d.GenericName,
                IsJanAushadhiGeneric = d.IsJanAushadhiGeneric,
                HsnCode = d.HsnCode,
                GstRatePct = d.GstRatePct,
                ScheduleFlag = d.ScheduleFlag.ToString(),
                UnitOfMeasure = d.UnitOfMeasure,
                IsActive = d.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}