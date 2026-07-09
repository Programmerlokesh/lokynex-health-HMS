using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.IcuMonitoring.Commands.RecordVitals;

public class RecordVitalsCommandHandler : IRequestHandler<RecordVitalsCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RecordVitalsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RecordVitalsCommand request, CancellationToken cancellationToken)
    {
        var admissionExists = await _context.IcuAdmissions
            .AnyAsync(a => a.Id == request.IcuAdmissionId, cancellationToken);

        if (!admissionExists)
        {
            throw new KeyNotFoundException($"ICU admission with Id '{request.IcuAdmissionId}' was not found.");
        }

        var breaches = new List<string>();

        if (request.HeartRate is < 40 or > 150) breaches.Add("heart_rate");
        if (request.SystolicBp is < 90 or > 180) breaches.Add("systolic_bp");
        if (request.MapMmHg is < 65) breaches.Add("map_mmhg");
        if (request.SpO2Pct is < 90) breaches.Add("spo2_pct");
        if (request.TemperatureC is < 35 or > 39) breaches.Add("temperature_c");
        if (request.UrineOutputMlPerKgHr is < 0.5m) breaches.Add("urine_output_ml_per_kg_hr");

        var vital = new IcuVital
        {
            IcuAdmissionId = request.IcuAdmissionId,
            HeartRate = request.HeartRate,
            SystolicBp = request.SystolicBp,
            DiastolicBp = request.DiastolicBp,
            MapMmHg = request.MapMmHg,
            SpO2Pct = request.SpO2Pct,
            TemperatureC = request.TemperatureC,
            UrineOutputMlPerKgHr = request.UrineOutputMlPerKgHr,
            RespiratoryRate = request.RespiratoryRate,
            IsThresholdBreach = breaches.Count > 0,
            BreachParameters = breaches
        };

        _context.IcuVitals.Add(vital);
        await _context.SaveChangesAsync(cancellationToken);

        return vital.Id;
    }
}