using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Laboratory.Commands.ReleaseLabOrder;

public class ReleaseLabOrderCommandHandler : IRequestHandler<ReleaseLabOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public ReleaseLabOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(ReleaseLabOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.LabOrders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException($"Lab order with Id '{request.OrderId}' was not found.");
        }

        if (order.Status == LabOrderStatus.Cancelled)
        {
            throw new InvalidOperationException("A cancelled lab order cannot be released.");
        }

        order.Status = LabOrderStatus.Released;
        order.ReleasedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}