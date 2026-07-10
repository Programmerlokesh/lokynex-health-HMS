using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.DoctorOpd.Commands.CreatePrescription;

public class CreatePrescriptionCommandHandler : IRequestHandler<CreatePrescriptionCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreatePrescriptionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        var visit = await _context.OpdVisits
            .FirstOrDefaultAsync(v => v.Id == request.VisitId, cancellationToken);

        if (visit is null)
        {
            throw new KeyNotFoundException($"OPD visit with Id '{request.VisitId}' was not found.");
        }

        if (visit.Status != RecordStatus.Active)
        {
            throw new InvalidOperationException("Cannot add a prescription to a visit that is not active.");
        }

        var prescriptionNumber = $"RX-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var prescription = new Prescription
        {
            PrescriptionNumber = prescriptionNumber,
            VisitId = visit.Id,
            PatientId = visit.PatientId,
            DoctorId = visit.DoctorId
        };

        _context.Prescriptions.Add(prescription);

        foreach (var item in request.Items)
        {
            _context.PrescriptionItems.Add(new PrescriptionItem
            {
                PrescriptionId = prescription.Id,
                DrugName = item.DrugName,
                RxnormCode = item.RxnormCode,
                Dosage = item.Dosage,
                Frequency = item.Frequency,
                DurationDays = item.DurationDays,
                ScheduleFlag = item.ScheduleFlag
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return prescription.Id;
    }
}