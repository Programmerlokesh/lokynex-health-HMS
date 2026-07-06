using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Doctors.DTOs;

namespace LokynexHealth.Application.Features.Doctors.Queries.GetAllDoctors;

public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllDoctorsQuery, List<DoctorDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllDoctorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DoctorDto>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Doctors
            .AsNoTracking()
            .Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.FullName,
                PhoneNumber = d.PhoneNumber,
                Email = d.Email,
                Specialization = d.Specialization.ToString(),
                RegistrationNumber = d.RegistrationNumber,
                IsAvailable = d.IsAvailable
            })
            .ToListAsync(cancellationToken);
    }
}
