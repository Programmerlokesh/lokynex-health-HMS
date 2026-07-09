using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.IcuMonitoring.DTOs;

namespace LokynexHealth.Application.Features.IcuMonitoring.Queries.GetIcuAdmissionById;

public class GetIcuAdmissionByIdQueryHandler : IRequestHandler<GetIcuAdmissionByIdQuery, IcuAdmissionDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetIcuAdmissionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IcuAdmissionDetailDto?> Handle(GetIcuAdmissionByIdQuery request, CancellationToken cancellationToken)
    {
        var admission = await _context.IcuAdmissions.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (admission is null)
        {
            return null;
        }

        var latestVitals = await _context.IcuVitals.AsNoTracking()
            .Where(v => v.IcuAdmissionId == admission.Id)
            .OrderByDescending(v => v.RecordedAt)
            .Take(20)
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

        return new IcuAdmissionDetailDto
        {
            Id = admission.Id,
            AdmissionId = admission.AdmissionId,
            IcuUnitType = admission.IcuUnitType.ToString(),
            Status = admission.Status.ToString(),
            AdmittedAt = admission.AdmittedAt,
            DischargedAt = admission.DischargedAt,
            ApacheIiScore = admission.ApacheIiScore,
            SofaScore = admission.SofaScore,
            LatestVitals = latestVitals
        };
    }
}