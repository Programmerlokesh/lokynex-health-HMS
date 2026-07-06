using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Patients.DTOs;

namespace LokynexHealth.Application.Features.Patients.Queries.GetAllPatients;

public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, List<PatientDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllPatientsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PatientDto>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Patients
            .AsNoTracking()
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FullName = p.FullName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender.ToString(),
                Mobile = p.Mobile,
                Email = p.Email,
                Address = p.Address,
                Uhid = p.Uhid
            })
            .ToListAsync(cancellationToken);
    }
}
