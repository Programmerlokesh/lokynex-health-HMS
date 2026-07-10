using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.DoctorOpd.DTOs;

namespace LokynexHealth.Application.Features.DoctorOpd.Queries.GetPrescriptionByVisitId;

public class GetPrescriptionByVisitIdQueryHandler : IRequestHandler<GetPrescriptionByVisitIdQuery, PrescriptionDto?>
{
    private readonly IApplicationDbContext _context;

    public GetPrescriptionByVisitIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PrescriptionDto?> Handle(GetPrescriptionByVisitIdQuery request, CancellationToken cancellationToken)
    {
        var prescription = await _context.Prescriptions.AsNoTracking()
            .Where(p => p.VisitId == request.VisitId)
            .OrderByDescending(p => p.IssuedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (prescription is null)
        {
            return null;
        }

        var items = await _context.PrescriptionItems.AsNoTracking()
            .Where(i => i.PrescriptionId == prescription.Id)
            .ToListAsync(cancellationToken);

        return new PrescriptionDto
        {
            Id = prescription.Id,
            PrescriptionNumber = prescription.PrescriptionNumber,
            VisitId = prescription.VisitId,
            PatientId = prescription.PatientId,
            DoctorId = prescription.DoctorId,
            IssuedAt = prescription.IssuedAt,
            PushedToAbha = prescription.PushedToAbha,
            PushedToPharmacyQueue = prescription.PushedToPharmacyQueue,
            Items = items.Select(i => new PrescriptionItemDto
            {
                Id = i.Id,
                DrugName = i.DrugName,
                RxnormCode = i.RxnormCode,
                Dosage = i.Dosage,
                Frequency = i.Frequency,
                DurationDays = i.DurationDays,
                ScheduleFlag = i.ScheduleFlag.ToString()
            }).ToList()
        };
    }
}