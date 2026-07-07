using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Laboratory.DTOs;

namespace LokynexHealth.Application.Features.Laboratory.Queries.GetPendingLabOrders;

public class GetPendingLabOrdersQueryHandler : IRequestHandler<GetPendingLabOrdersQuery, List<LabOrderSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingLabOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LabOrderSummaryDto>> Handle(GetPendingLabOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _context.LabOrders.AsNoTracking()
            .Where(o => o.Status != LabOrderStatus.Released && o.Status != LabOrderStatus.Cancelled)
            .OrderBy(o => o.Priority)
            .ThenBy(o => o.OrderedAt)
            .Select(o => new LabOrderSummaryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                PatientId = o.PatientId,
                Status = o.Status.ToString(),
                Priority = o.Priority.ToString(),
                OrderedAt = o.OrderedAt
            })
            .ToListAsync(cancellationToken);
    }
}