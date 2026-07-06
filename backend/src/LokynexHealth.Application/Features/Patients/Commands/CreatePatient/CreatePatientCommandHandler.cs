using MediatR;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreatePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var uhid = $"WB-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var patient = new Patient
        {
            FullName = request.FullName,
            DateOfBirth = DateOnly.FromDateTime(request.DateOfBirth),
            Gender = request.Gender,
            Mobile = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address,
            Uhid = uhid
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);

        return patient.Id;
    }
}
