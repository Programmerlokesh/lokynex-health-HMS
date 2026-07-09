using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.DoctorOpd.Commands.CreateOpdVisit;

public class CreateOpdVisitCommandHandler : IRequestHandler<CreateOpdVisitCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateOpdVisitCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateOpdVisitCommand request, CancellationToken cancellationToken)
    {
        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

        if (!patientExists)
        {
            throw new KeyNotFoundException($"Patient with Id '{request.PatientId}' was not found.");
        }

        var doctorExists = await _context.Doctors
            .AnyAsync(d => d.Id == request.DoctorId, cancellationToken);

        if (!doctorExists)
        {
            throw new KeyNotFoundException($"Doctor with Id '{request.DoctorId}' was not found.");
        }

        OpdTokenQueue? token = null;

        if (request.TokenId.HasValue)
        {
            token = await _context.OpdTokenQueue
                .FirstOrDefaultAsync(t => t.Id == request.TokenId.Value, cancellationToken);

            if (token is null)
            {
                throw new KeyNotFoundException($"Token with Id '{request.TokenId}' was not found.");
            }

            if (token.Status is not ("waiting" or "called"))
            {
                throw new InvalidOperationException($"Token '{token.TokenNumber}' is not available for a visit (current status: {token.Status}).");
            }
        }

        var visit = new OpdVisit
        {
            VisitNumber = $"OPD-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            TokenId = request.TokenId,
            SchemeTag = request.SchemeTag,
            ChiefComplaint = request.ChiefComplaint
        };

        if (token is not null)
        {
            token.Status = "in_consultation";
            token.CalledAt ??= DateTimeOffset.UtcNow;
        }

        _context.OpdVisits.Add(visit);
        await _context.SaveChangesAsync(cancellationToken);

        return visit.Id;
    }
}
