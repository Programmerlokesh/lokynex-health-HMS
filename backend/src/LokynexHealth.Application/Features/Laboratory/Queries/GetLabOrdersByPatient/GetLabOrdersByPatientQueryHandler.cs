using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Laboratory.DTOs;

namespace LokynexHealth.Application.Features.Laboratory.Queries.GetLabOrdersByPatient;

public class GetLabOrdersByPatientQueryHandler : IRequestHandler<GetLabOrdersByPatientQuery, List<LabOrderSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLabOrdersByPatientQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LabOrderSummaryDto>> Handle(GetLabOrdersByPatientQuery request, CancellationToken cancellationToken)
    {
        return await _context.LabOrders.AsNoTracking()
            .Where(o => o.PatientId == request.PatientId)
            .OrderByDescending(o => o.OrderedAt)
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