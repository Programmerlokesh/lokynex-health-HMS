using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Laboratory.DTOs;

namespace LokynexHealth.Application.Features.Laboratory.Queries.GetAllLabTests;

public class GetAllLabTestsQueryHandler : IRequestHandler<GetAllLabTestsQuery, List<LabTestDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllLabTestsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LabTestDto>> Handle(GetAllLabTestsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.LabTestCatalog.AsNoTracking().AsQueryable();

        if (!request.IncludeInactive)
        {
            query = query.Where(t => t.IsActive);
        }

        return await query
            .Select(t => new LabTestDto
            {
                Id = t.Id,
                TestCode = t.TestCode,
                TestName = t.TestName,
                SpecimenType = t.SpecimenType,
                StandardPrice = t.StandardPrice,
                TatHoursStd = t.TatHoursStd,
                IsActive = t.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}