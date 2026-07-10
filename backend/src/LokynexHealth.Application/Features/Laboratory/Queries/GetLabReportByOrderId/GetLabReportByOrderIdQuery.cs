using MediatR;
using LokynexHealth.Application.Features.Laboratory.DTOs;

namespace LokynexHealth.Application.Features.Laboratory.Queries.GetLabReportByOrderId;

public class GetLabReportByOrderIdQuery : IRequest<LabReportDto?>
{
    public Guid OrderId { get; set; }
}