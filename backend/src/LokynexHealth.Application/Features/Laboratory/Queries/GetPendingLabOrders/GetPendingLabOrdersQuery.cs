using MediatR;
using LokynexHealth.Application.Features.Laboratory.DTOs;

namespace LokynexHealth.Application.Features.Laboratory.Queries.GetPendingLabOrders;

public class GetPendingLabOrdersQuery : IRequest<List<LabOrderSummaryDto>>
{
}