using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Laboratory.Commands.CollectSample;

public class CollectSampleCommandHandler : IRequestHandler<CollectSampleCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CollectSampleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CollectSampleCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.LabOrders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException($"Lab order with Id '{request.OrderId}' was not found.");
        }

        var sample = new LabSample
        {
            OrderId = request.OrderId,
            SampleBarcode = request.SampleBarcode,
            CollectedBy = request.CollectedBy,
            CollectedAt = DateTimeOffset.UtcNow
        };

        order.Status = LabOrderStatus.SampleCollected;

        _context.LabSamples.Add(sample);
        await _context.SaveChangesAsync(cancellationToken);

        return sample.Id;
    }
}