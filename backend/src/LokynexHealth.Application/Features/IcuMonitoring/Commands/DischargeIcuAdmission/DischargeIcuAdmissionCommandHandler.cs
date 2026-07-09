using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.DischargeIcuAdmission;

public class DischargeIcuAdmissionCommandHandler : IRequestHandler<DischargeIcuAdmissionCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public DischargeIcuAdmissionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(DischargeIcuAdmissionCommand request, CancellationToken cancellationToken)
    {
        var admission = await _context.IcuAdmissions
            .FirstOrDefaultAsync(a => a.Id == request.IcuAdmissionId, cancellationToken);

        if (admission is null)
        {
            throw new KeyNotFoundException($"ICU admission with Id '{request.IcuAdmissionId}' was not found.");
        }

        if (admission.Status != RecordStatus.Active)
        {
            throw new InvalidOperationException("ICU admission is already discharged.");
        }

        admission.Status = RecordStatus.Completed;
        admission.DischargedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return admission.Id;
    }
}