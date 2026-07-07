using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.WardBed.DTOs;

namespace LokynexHealth.Application.Features.WardBed.Queries.GetAdmissionsByPatient;

public class GetAdmissionsByPatientQueryHandler : IRequestHandler<GetAdmissionsByPatientQuery, List<AdmissionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAdmissionsByPatientQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdmissionDto>> Handle(GetAdmissionsByPatientQuery request, CancellationToken cancellationToken)
    {
        return await _context.Admissions
            .AsNoTracking()
            .Where(a => a.PatientId == request.PatientId)
            .OrderByDescending(a => a.AdmittedAt)
            .Select(a => new AdmissionDto
            {
                Id = a.Id,
                AdmissionNumber = a.AdmissionNumber,
                PatientId = a.PatientId,
                AdmittingDoctorId = a.AdmittingDoctorId,
                BedId = a.BedId,
                PmjayPackageCode = a.PmjayPackageCode,
                AdmittedAt = a.AdmittedAt,
                DischargedAt = a.DischargedAt,
                Status = a.Status.ToString()
            })
            .ToListAsync(cancellationToken);
    }
}