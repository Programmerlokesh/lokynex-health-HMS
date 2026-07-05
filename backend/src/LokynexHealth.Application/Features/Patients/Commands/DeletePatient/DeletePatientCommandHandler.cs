using MediatR;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Patients.Commands.DeletePatient;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeletePatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _context.Patients.FindAsync(new object[] { request.Id }, cancellationToken);

        if (patient is null)
        {
            throw new KeyNotFoundException($"Patient with Id '{request.Id}' was not found.");
        }

        patient.IsDeleted = true;
        patient.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
