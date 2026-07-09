using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Icu.DTOs;

namespace LokynexHealth.Application.Features.Icu.Commands.RecordVitals;

public class RecordVitalsCommandHandler : IRequestHandler<RecordVitalsCommand, IcuVitalDto>
{
    private readonly IApplicationDbContext _context;

    public RecordVitalsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IcuVitalDto> Handle(RecordVitalsCommand request, CancellationToken cancellationToken)
    {
        var icuAdmissionExists = await _context.IcuAdmissions
            .AnyAsync(i => i.Id == request.IcuAdmissionId, cancellationToken);

        if (!icuAdmissionExists)
        {
            throw new KeyNotFoundException($"ICU admission with Id '{request.IcuAdmissionId}' was not found.");
        }

        var breaches = new List<string>();

        // Clinical alert thresholds — ref: docs/db-schema-reference/07_icu_monitoring.sql
        if (request.HeartRate is < 40 or > 150) breaches.Add("heart_rate");
        if (request.SystolicBp is < 90 or > 180) breaches.Add("systolic_bp");
        if (request.MapMmhg is < 65) breaches.Add("map_mmhg");
        if (request.Spo2Pct is < 90) breaches.Add("spo2_pct");
        if (request.TemperatureC is < 35 or > 39) breaches.Add("temperature_c");
        if (request.UrineOutputMlPerKgHr is < 0.5m) breaches.Add("urine_output_ml_per_kg_hr");

        var vital = new IcuVital
        {
            IcuAdmissionId = request.IcuAdmissionId,
            HeartRate = request.HeartRate,
            SystolicBp = request.SystolicBp,
            DiastolicBp = request.DiastolicBp,
            MapMmHg = request.MapMmhg,
            SpO2Pct = request.Spo2Pct,
            TemperatureC = request.TemperatureC,
            UrineOutputMlPerKgHr = request.UrineOutputMlPerKgHr,
            RespiratoryRate = request.RespiratoryRate,
            IsThresholdBreach = breaches.Count > 0,
            BreachParameters = breaches
        };

        _context.IcuVitals.Add(vital);
        await _context.SaveChangesAsync(cancellationToken);

        return new IcuVitalDto
        {
            Id = vital.Id,
            IcuAdmissionId = vital.IcuAdmissionId,
            RecordedAt = vital.RecordedAt,
            HeartRate = vital.HeartRate,
            SystolicBp = vital.SystolicBp,
            DiastolicBp = vital.DiastolicBp,
            MapMmhg = vital.MapMmHg,
            Spo2Pct = vital.SpO2Pct,
            TemperatureC = vital.TemperatureC,
            UrineOutputMlPerKgHr = vital.UrineOutputMlPerKgHr,
            RespiratoryRate = vital.RespiratoryRate,
            IsThresholdBreach = vital.IsThresholdBreach,
            BreachParameters = vital.BreachParameters
        };
    }
}