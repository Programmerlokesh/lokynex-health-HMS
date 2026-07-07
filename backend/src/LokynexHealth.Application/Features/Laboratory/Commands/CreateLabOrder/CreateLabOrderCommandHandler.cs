using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Laboratory.Commands.CreateLabOrder;

public class CreateLabOrderCommandHandler : IRequestHandler<CreateLabOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateLabOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateLabOrderCommand request, CancellationToken cancellationToken)
    {
        var patientExists = await _context.Patients
            .AnyAsync(p => p.Id == request.PatientId, cancellationToken);

        if (!patientExists)
        {
            throw new KeyNotFoundException($"Patient with Id '{request.PatientId}' was not found.");
        }

        var tests = await _context.LabTestCatalog
            .Where(t => request.TestIds.Contains(t.Id) && t.IsActive)
            .ToListAsync(cancellationToken);

        if (tests.Count != request.TestIds.Distinct().Count())
        {
            throw new KeyNotFoundException("One or more requested lab tests were not found or are inactive.");
        }

        var order = new LabOrder
        {
            OrderNumber = $"LAB-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            PatientId = request.PatientId,
            OrderingDoctorId = request.OrderingDoctorId,
            SourceVisitId = request.SourceVisitId,
            Priority = request.Priority,
            SchemeTag = request.SchemeTag
        };

        _context.LabOrders.Add(order);

        foreach (var test in tests)
        {
            _context.LabOrderTests.Add(new LabOrderTest
            {
                OrderId = order.Id,
                TestId = test.Id,
                PriceApplied = test.StandardPrice
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}