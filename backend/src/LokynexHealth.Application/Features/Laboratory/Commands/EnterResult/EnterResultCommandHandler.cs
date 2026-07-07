using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Laboratory.Commands.EnterResult;

public class EnterResultCommandHandler : IRequestHandler<EnterResultCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public EnterResultCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(EnterResultCommand request, CancellationToken cancellationToken)
    {
        var orderTest = await _context.LabOrderTests
            .FirstOrDefaultAsync(t => t.Id == request.OrderTestId, cancellationToken);

        if (orderTest is null)
        {
            throw new KeyNotFoundException($"Lab order test with Id '{request.OrderTestId}' was not found.");
        }

        var order = await _context.LabOrders
            .FirstOrDefaultAsync(o => o.Id == orderTest.OrderId, cancellationToken);

        var result = new LabResult
        {
            OrderTestId = request.OrderTestId,
            SampleId = request.SampleId,
            ParameterName = request.ParameterName,
            ResultValue = request.ResultValue,
            Unit = request.Unit,
            ReferenceRange = request.ReferenceRange,
            IsCritical = request.IsCritical,
            IsAbnormal = request.IsAbnormal,
            EnteredBy = request.EnteredBy,
            Source = "manual"
        };

        _context.LabResults.Add(result);

        if (order is not null && order.Status is LabOrderStatus.Ordered or LabOrderStatus.SampleCollected)
        {
            order.Status = LabOrderStatus.InAnalysis;
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (request.IsCritical && order is not null)
        {
            _context.LabCriticalAlerts.Add(new LabCriticalAlert
            {
                ResultId = result.Id,
                NotifiedDoctorId = order.OrderingDoctorId,
                NotifiedVia = "sms",
                SentAt = DateTimeOffset.UtcNow
            });

            await _context.SaveChangesAsync(cancellationToken);
        }

        return result.Id;
    }
}