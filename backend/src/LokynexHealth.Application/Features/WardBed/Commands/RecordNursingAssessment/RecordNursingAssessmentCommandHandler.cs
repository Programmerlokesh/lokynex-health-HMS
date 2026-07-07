using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.WardBed.Commands.RecordNursingAssessment;

public class RecordNursingAssessmentCommandHandler : IRequestHandler<RecordNursingAssessmentCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RecordNursingAssessmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RecordNursingAssessmentCommand request, CancellationToken cancellationToken)
    {
        var admissionExists = await _context.Admissions
            .AnyAsync(a => a.Id == request.AdmissionId, cancellationToken);

        if (!admissionExists)
        {
            throw new KeyNotFoundException($"Admission with Id '{request.AdmissionId}' was not found.");
        }

        var assessment = new NursingAssessment
        {
            AdmissionId = request.AdmissionId,
            AssessedBy = request.AssessedBy,
            VitalsJson = request.VitalsJson,
            GcsScore = request.GcsScore,
            BradenScore = request.BradenScore,
            MorseFallScore = request.MorseFallScore,
            Nrs2002Score = request.Nrs2002Score
        };

        _context.NursingAssessments.Add(assessment);
        await _context.SaveChangesAsync(cancellationToken);

        return assessment.Id;
    }
}