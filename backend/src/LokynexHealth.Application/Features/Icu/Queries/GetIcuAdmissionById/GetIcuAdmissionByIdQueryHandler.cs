using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Icu.DTOs;

namespace LokynexHealth.Application.Features.Icu.Queries.GetIcuAdmissionById;

public class GetIcuAdmissionByIdQueryHandler : IRequestHandler<GetIcuAdmissionByIdQuery, IcuAdmissionDto?>
{
    private readonly IApplicationDbContext _context;

    public GetIcuAdmissionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IcuAdmissionDto?> Handle(GetIcuAdmissionByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.IcuAdmissions
            .AsNoTracking()
            .Where(i => i.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}