using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.IcuMonitoring.DTOs;

namespace LokynexHealth.Application.Features.IcuMonitoring.Queries.GetVitalsByAdmission;

public class GetVitalsByAdmissionQueryHandler : IRequestHandler<GetVitalsByAdmissionQuery, List<IcuVitalDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVitalsByAdmissionQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IcuVitalDto>> Handle(GetVitalsByAdmissionQuery request, CancellationToken cancellationToken)
    {
        return await _context.IcuVitals.AsNoTracking()
            .Where(v => v.IcuAdmissionId == request.IcuAdmissionId)
            .OrderByDescending(v => v.RecordedAt)
            .Select(v => new IcuVitalDto
            {
                Id = v.Id,
                RecordedAt = v.RecordedAt,
                HeartRate = v.HeartRate,
                SystolicBp = v.SystolicBp,
                DiastolicBp = v.DiastolicBp,
                MapMmHg = v.MapMmHg,
                SpO2Pct = v.SpO2Pct,
                TemperatureC = v.TemperatureC,
                UrineOutputMlPerKgHr = v.UrineOutputMlPerKgHr,
                RespiratoryRate = v.RespiratoryRate,
                IsThresholdBreach = v.IsThresholdBreach,
                BreachParameters = v.BreachParameters
            })
            .ToListAsync(cancellationToken);
    }
}