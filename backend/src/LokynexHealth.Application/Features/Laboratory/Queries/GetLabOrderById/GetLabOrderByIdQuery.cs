using MediatR;
using LokynexHealth.Application.Features.Laboratory.DTOs;

namespace LokynexHealth.Application.Features.Laboratory.Queries.GetLabOrderById;

public class GetLabOrderByIdQuery : IRequest<LabOrderDetailDto?>
{
    public Guid Id { get; set; }
}