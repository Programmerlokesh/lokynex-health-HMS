using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.CreateIcuAdmission;

public class CreateIcuAdmissionCommandHandler : IRequestHandler<CreateIcuAdmissionCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateIcuAdmissionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateIcuAdmissionCommand request, CancellationToken cancellationToken)
    {
        var admissionExists = await _context.Admissions
            .AnyAsync(a => a.Id == request.AdmissionId, cancellationToken);

        if (!admissionExists)
        {
            throw new KeyNotFoundException($"Admission with Id '{request.AdmissionId}' was not found.");
        }

        var icuAdmission = new IcuAdmission
        {
            AdmissionId = request.AdmissionId,
            IcuUnitType = request.IcuUnitType,
            ApacheIiScore = request.ApacheIiScore,
            SofaScore = request.SofaScore
        };

        _context.IcuAdmissions.Add(icuAdmission);
        await _context.SaveChangesAsync(cancellationToken);

        return icuAdmission.Id;
    }
}