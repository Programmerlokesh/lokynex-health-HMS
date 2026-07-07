using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Laboratory.DTOs;

namespace LokynexHealth.Application.Features.Laboratory.Queries.GetLabOrderById;

public class GetLabOrderByIdQueryHandler : IRequestHandler<GetLabOrderByIdQuery, LabOrderDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetLabOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LabOrderDetailDto?> Handle(GetLabOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.LabOrders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            return null;
        }

        var orderTests = await _context.LabOrderTests.AsNoTracking()
            .Where(t => t.OrderId == order.Id)
            .ToListAsync(cancellationToken);

        var testIds = orderTests.Select(t => t.Id).ToList();
        var catalogNames = await _context.LabTestCatalog.AsNoTracking()
            .Where(c => orderTests.Select(t => t.TestId).Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.TestName, cancellationToken);

        var results = await _context.LabResults.AsNoTracking()
            .Where(r => testIds.Contains(r.OrderTestId))
            .ToListAsync(cancellationToken);

        return new LabOrderDetailDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            PatientId = order.PatientId,
            OrderingDoctorId = order.OrderingDoctorId,
            Status = order.Status.ToString(),
            Priority = order.Priority.ToString(),
            SchemeTag = order.SchemeTag,
            OrderedAt = order.OrderedAt,
            ReleasedAt = order.ReleasedAt,
            Tests = orderTests.Select(t => new LabOrderTestDto
            {
                Id = t.Id,
                TestId = t.TestId,
                TestName = catalogNames.GetValueOrDefault(t.TestId, string.Empty),
                PriceApplied = t.PriceApplied,
                Results = results.Where(r => r.OrderTestId == t.Id)
                    .Select(r => new LabResultDto
                    {
                        Id = r.Id,
                        ParameterName = r.ParameterName,
                        ResultValue = r.ResultValue,
                        Unit = r.Unit,
                        ReferenceRange = r.ReferenceRange,
                        IsCritical = r.IsCritical,
                        IsAbnormal = r.IsAbnormal,
                        ValidatedAt = r.ValidatedAt
                    }).ToList()
            }).ToList()
        };
    }
}