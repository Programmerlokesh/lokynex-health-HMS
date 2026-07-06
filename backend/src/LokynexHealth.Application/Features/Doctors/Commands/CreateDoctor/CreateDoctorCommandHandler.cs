using MediatR;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateDoctorCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = new Doctor
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Specialization = request.Specialization,
            RegistrationNumber = request.RegistrationNumber,
            IsAvailable = true
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync(cancellationToken);

        return doctor.Id;
    }
}
