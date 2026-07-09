using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.WardBed.DTOs;

namespace LokynexHealth.Application.Features.WardBed.Queries.GetAdmissionById;

public class GetAdmissionByIdQueryHandler : IRequestHandler<GetAdmissionByIdQuery, AdmissionDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAdmissionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdmissionDto?> Handle(GetAdmissionByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Admissions
            .AsNoTracking()
            .Where(a => a.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}