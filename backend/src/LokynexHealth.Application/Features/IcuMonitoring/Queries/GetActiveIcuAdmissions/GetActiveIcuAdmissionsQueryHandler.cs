using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.IcuMonitoring.DTOs;

namespace LokynexHealth.Application.Features.IcuMonitoring.Queries.GetActiveIcuAdmissions;

public class GetActiveIcuAdmissionsQueryHandler : IRequestHandler<GetActiveIcuAdmissionsQuery, List<IcuAdmissionSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetActiveIcuAdmissionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IcuAdmissionSummaryDto>> Handle(GetActiveIcuAdmissionsQuery request, CancellationToken cancellationToken)
    {
        return await _context.IcuAdmissions.AsNoTracking()
            .Where(a => a.Status == RecordStatus.Active)
            .OrderByDescending(a => a.AdmittedAt)
            .Select(a => new IcuAdmissionSummaryDto
            {
                Id = a.Id,
                AdmissionId = a.AdmissionId,
                IcuUnitType = a.IcuUnitType.ToString(),
                Status = a.Status.ToString(),
                AdmittedAt = a.AdmittedAt,
                ApacheIiScore = a.ApacheIiScore,
                SofaScore = a.SofaScore
            })
            .ToListAsync(cancellationToken);
    }
}