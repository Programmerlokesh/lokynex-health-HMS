using MediatR;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdatePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _context.Patients.FindAsync(new object[] { request.Id }, cancellationToken);

        if (patient is null)
        {
            throw new KeyNotFoundException($"Patient with Id '{request.Id}' was not found.");
        }

        patient.FullName = request.FullName;
        patient.DateOfBirth = DateOnly.FromDateTime(request.DateOfBirth);
        patient.Gender = request.Gender;
        patient.Mobile = request.PhoneNumber;
        patient.Email = request.Email;
        patient.Address = request.Address;
        patient.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
