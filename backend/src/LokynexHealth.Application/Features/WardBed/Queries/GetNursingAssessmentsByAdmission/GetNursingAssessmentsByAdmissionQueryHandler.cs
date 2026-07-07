using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.WardBed.DTOs;

namespace LokynexHealth.Application.Features.WardBed.Queries.GetNursingAssessmentsByAdmission;

public class GetNursingAssessmentsByAdmissionQueryHandler
    : IRequestHandler<GetNursingAssessmentsByAdmissionQuery, List<NursingAssessmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetNursingAssessmentsByAdmissionQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<NursingAssessmentDto>> Handle(GetNursingAssessmentsByAdmissionQuery request, CancellationToken cancellationToken)
    {
        return await _context.NursingAssessments
            .AsNoTracking()
            .Where(n => n.AdmissionId == request.AdmissionId)
            .OrderByDescending(n => n.AssessedAt)
            .Select(n => new NursingAssessmentDto
            {
                Id = n.Id,
                AdmissionId = n.AdmissionId,
                AssessedBy = n.AssessedBy,
                VitalsJson = n.VitalsJson,
                GcsScore = n.GcsScore,
                BradenScore = n.BradenScore,
                MorseFallScore = n.MorseFallScore,
                Nrs2002Score = n.Nrs2002Score,
                AssessedAt = n.AssessedAt
            })
            .ToListAsync(cancellationToken);
    }
}