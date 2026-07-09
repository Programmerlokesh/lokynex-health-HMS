using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.RecordVentilatorReading;

public class RecordVentilatorReadingCommandHandler : IRequestHandler<RecordVentilatorReadingCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RecordVentilatorReadingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RecordVentilatorReadingCommand request, CancellationToken cancellationToken)
    {
        var admissionExists = await _context.IcuAdmissions
            .AnyAsync(a => a.Id == request.IcuAdmissionId, cancellationToken);

        if (!admissionExists)
        {
            throw new KeyNotFoundException($"ICU admission with Id '{request.IcuAdmissionId}' was not found.");
        }

        var record = new IcuVentilatorRecord
        {
            IcuAdmissionId = request.IcuAdmissionId,
            Mode = request.Mode,
            Fio2Pct = request.Fio2Pct,
            PeepCmH2O = request.PeepCmH2O,
            TidalVolumeMl = request.TidalVolumeMl
        };

        _context.IcuVentilatorRecords.Add(record);
        await _context.SaveChangesAsync(cancellationToken);

        return record.Id;
    }
}