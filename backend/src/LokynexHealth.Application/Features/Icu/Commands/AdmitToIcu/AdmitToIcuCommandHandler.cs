using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Icu.Commands.AdmitToIcu;

public class AdmitToIcuCommandHandler : IRequestHandler<AdmitToIcuCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AdmitToIcuCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AdmitToIcuCommand request, CancellationToken cancellationToken)
    {
        var admission = await _context.Admissions
            .FirstOrDefaultAsync(a => a.Id == request.AdmissionId, cancellationToken);

        if (admission is null)
        {
            throw new KeyNotFoundException($"Admission with Id '{request.AdmissionId}' was not found.");
        }

        if (admission.Status != RecordStatus.Active)
        {
            throw new InvalidOperationException("Cannot open an ICU episode for an admission that is not active.");
        }

        var alreadyInIcu = await _context.IcuAdmissions
            .AnyAsync(i => i.AdmissionId == request.AdmissionId && i.Status == RecordStatus.Active, cancellationToken);

        if (alreadyInIcu)
        {
            throw new InvalidOperationException("This admission already has an active ICU episode.");
        }

        var icuAdmission = new IcuAdmission
        {
            AdmissionId = request.AdmissionId,
            IcuUnitType = request.IcuUnitType,
            ApacheIiScore = request.ApacheIiScore,
            SofaScore = request.SofaScore,
            Status = RecordStatus.Active
        };

        _context.IcuAdmissions.Add(icuAdmission);
        await _context.SaveChangesAsync(cancellationToken);

        return icuAdmission.Id;
    }
}