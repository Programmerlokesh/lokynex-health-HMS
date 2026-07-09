using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Icu.DTOs;

namespace LokynexHealth.Application.Features.Icu.Queries.GetVitalsByIcuAdmission;

public class GetVitalsByIcuAdmissionQueryHandler : IRequestHandler<GetVitalsByIcuAdmissionQuery, List<IcuVitalDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVitalsByIcuAdmissionQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IcuVitalDto>> Handle(GetVitalsByIcuAdmissionQuery request, CancellationToken cancellationToken)
    {
        return await _context.IcuVitals
            .AsNoTracking()
            .Where(v => v.IcuAdmissionId == request.IcuAdmissionId)
            .OrderByDescending(v => v.RecordedAt)
            .Select(v => new IcuVitalDto
            {
                Id = v.Id,
                IcuAdmissionId = v.IcuAdmissionId,
                RecordedAt = v.RecordedAt,
                HeartRate = v.HeartRate,
                SystolicBp = v.SystolicBp,
                DiastolicBp = v.DiastolicBp,
                MapMmhg = v.MapMmhg,
                Spo2Pct = v.Spo2Pct,
                TemperatureC = v.TemperatureC,
                UrineOutputMlPerKgHr = v.UrineOutputMlPerKgHr,
                RespiratoryRate = v.RespiratoryRate,
                IsThresholdBreach = v.IsThresholdBreach,
                BreachParameters = v.BreachParameters
            })
            .ToListAsync(cancellationToken);
    }
}