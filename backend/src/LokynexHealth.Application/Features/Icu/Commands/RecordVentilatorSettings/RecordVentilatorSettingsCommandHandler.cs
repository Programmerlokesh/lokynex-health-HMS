using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Icu.Commands.RecordVentilatorSettings;

public class RecordVentilatorSettingsCommandHandler : IRequestHandler<RecordVentilatorSettingsCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RecordVentilatorSettingsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RecordVentilatorSettingsCommand request, CancellationToken cancellationToken)
    {
        var icuAdmissionExists = await _context.IcuAdmissions
            .AnyAsync(i => i.Id == request.IcuAdmissionId, cancellationToken);

        if (!icuAdmissionExists)
        {
            throw new KeyNotFoundException($"ICU admission with Id '{request.IcuAdmissionId}' was not found.");
        }

        var record = new IcuVentilatorRecord
        {
            IcuAdmissionId = request.IcuAdmissionId,
            Mode = request.Mode,
            Fio2Pct = request.Fio2Pct,
            PeepCmh2o = request.PeepCmh2o,
            TidalVolumeMl = request.TidalVolumeMl
        };

        _context.IcuVentilatorRecords.Add(record);
        await _context.SaveChangesAsync(cancellationToken);

        return record.Id;
    }
}