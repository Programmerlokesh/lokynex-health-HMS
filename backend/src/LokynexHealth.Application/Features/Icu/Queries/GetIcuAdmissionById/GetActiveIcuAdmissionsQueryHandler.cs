using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Icu.DTOs;

namespace LokynexHealth.Application.Features.Icu.Queries.GetActiveIcuAdmissions;

public class GetActiveIcuAdmissionsQueryHandler : IRequestHandler<GetActiveIcuAdmissionsQuery, List<IcuAdmissionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetActiveIcuAdmissionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IcuAdmissionDto>> Handle(GetActiveIcuAdmissionsQuery request, CancellationToken cancellationToken)
    {
        return await _context.IcuAdmissions
            .AsNoTracking()
            .Where(i => i.Status == RecordStatus.Active)
            .OrderByDescending(i => i.AdmittedAt)
            .Select(i => new IcuAdmissionDto
            {
                Id = i.Id,
                AdmissionId = i.AdmissionId,
                IcuUnitType = i.IcuUnitType.ToString(),
                AdmittedAt = i.AdmittedAt,
                DischargedAt = i.DischargedAt,
                ApacheIiScore = i.ApacheIiScore,
                SofaScore = i.SofaScore,
                Status = i.Status.ToString()
            })
            .ToListAsync(cancellationToken);
    }
}
