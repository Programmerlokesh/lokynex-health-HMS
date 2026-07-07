using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.WardBed.Commands.AdmitPatient;

public class AdmitPatientCommandHandler : IRequestHandler<AdmitPatientCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AdmitPatientCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(AdmitPatientCommand request, CancellationToken cancellationToken)
    {
        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

        if (!patientExists)
        {
            throw new KeyNotFoundException($"Patient with Id '{request.PatientId}' was not found.");
        }

        if (request.AdmittingDoctorId.HasValue)
        {
            var doctorExists = await _context.Doctors
                .AnyAsync(d => d.Id == request.AdmittingDoctorId.Value, cancellationToken);

            if (!doctorExists)
            {
                throw new KeyNotFoundException($"Doctor with Id '{request.AdmittingDoctorId}' was not found.");
            }
        }

        var bed = await _context.Beds
            .FirstOrDefaultAsync(b => b.Id == request.BedId, cancellationToken);

        if (bed is null)
        {
            throw new KeyNotFoundException($"Bed with Id '{request.BedId}' was not found.");
        }

        if (bed.Status != BedStatus.Available)
        {
            throw new InvalidOperationException($"Bed '{bed.BedNumber}' is not available (current status: {bed.Status}).");
        }

        var admission = new Admission
        {
            AdmissionNumber = $"ADM-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            PatientId = request.PatientId,
            AdmittingDoctorId = request.AdmittingDoctorId,
            BedId = request.BedId,
            PmjayPackageCode = request.PmjayPackageCode,
            Status = RecordStatus.Active
        };

        bed.Status = BedStatus.Occupied;

        _context.Admissions.Add(admission);
        await _context.SaveChangesAsync(cancellationToken);

        return admission.Id;
    }
}