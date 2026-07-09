using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Icu.Commands.DischargeFromIcu;

public class DischargeFromIcuCommandHandler : IRequestHandler<DischargeFromIcuCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public DischargeFromIcuCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(DischargeFromIcuCommand request, CancellationToken cancellationToken)
    {
        var icuAdmission = await _context.IcuAdmissions
            .FirstOrDefaultAsync(i => i.Id == request.IcuAdmissionId, cancellationToken);

        if (icuAdmission is null)
        {
            throw new KeyNotFoundException($"ICU admission with Id '{request.IcuAdmissionId}' was not found.");
        }

        if (icuAdmission.Status != RecordStatus.Active)
        {
            throw new InvalidOperationException("This ICU episode is already closed.");
        }

        icuAdmission.DischargedAt = DateTimeOffset.UtcNow;
        icuAdmission.Status = RecordStatus.Completed;

        await _context.SaveChangesAsync(cancellationToken);

        return icuAdmission.Id;
    }
}