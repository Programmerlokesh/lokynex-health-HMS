using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Laboratory.DTOs;

namespace LokynexHealth.Application.Features.Laboratory.Queries.GetLabReportByOrderId;

public class GetLabReportByOrderIdQueryHandler : IRequestHandler<GetLabReportByOrderIdQuery, LabReportDto?>
{
    private readonly IApplicationDbContext _context;

    public GetLabReportByOrderIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LabReportDto?> Handle(GetLabReportByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.LabOrders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return null;
        }

        var patient = await _context.Patients.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == order.PatientId, cancellationToken);

        if (patient is null)
        {
            throw new KeyNotFoundException($"Patient with Id '{order.PatientId}' was not found.");
        }

        var doctor = order.OrderingDoctorId.HasValue
            ? await _context.Doctors.AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == order.OrderingDoctorId.Value, cancellationToken)
            : null;

        var orderTests = await _context.LabOrderTests.AsNoTracking()
            .Where(t => t.OrderId == order.Id)
            .ToListAsync(cancellationToken);

        var testIds = orderTests.Select(t => t.Id).ToList();
        var catalogTestIds = orderTests.Select(t => t.TestId).ToList();

        var catalog = await _context.LabTestCatalog.AsNoTracking()
            .Where(c => catalogTestIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c, cancellationToken);

        var results = await _context.LabResults.AsNoTracking()
            .Where(r => testIds.Contains(r.OrderTestId))
            .ToListAsync(cancellationToken);

        int? age = null;
        if (patient.DateOfBirth.HasValue)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var dob = patient.DateOfBirth.Value;
            age = today.Year - dob.Year;
            if (dob > today.AddYears(-age.Value))
            {
                age--;
            }
        }

        return new LabReportDto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            OrderedAt = order.OrderedAt,
            ReleasedAt = order.ReleasedAt,
            PatientId = patient.Id,
            PatientName = patient.FullName,
            PatientUhid = patient.Uhid,
            PatientAge = age,
            PatientGender = patient.Gender.ToString(),
            OrderingDoctorId = doctor?.Id,
            OrderingDoctorName = doctor?.FullName,
            OrderingDoctorRegistrationNo = doctor?.RegistrationNumber,
            Tests = orderTests.Select(t => new LabReportTestDto
            {
                TestName = catalog.TryGetValue(t.TestId, out var c) ? c.TestName : string.Empty,
                SpecimenType = catalog.TryGetValue(t.TestId, out var c2) ? c2.SpecimenType : null,
                Results = results.Where(r => r.OrderTestId == t.Id)
                    .Select(r => new LabReportResultDto
                    {
                        ParameterName = r.ParameterName,
                        ResultValue = r.ResultValue,
                        Unit = r.Unit,
                        ReferenceRange = r.ReferenceRange,
                        IsAbnormal = r.IsAbnormal,
                        IsCritical = r.IsCritical,
                        ValidatedAt = r.ValidatedAt
                    }).ToList()
            }).ToList()
        };
    }
}