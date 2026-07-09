using MediatR;
using LokynexHealth.Application.Features.IcuMonitoring.DTOs;

namespace LokynexHealth.Application.Features.IcuMonitoring.Queries.GetIcuAdmissionById;

public class GetIcuAdmissionByIdQuery : IRequest<IcuAdmissionDetailDto?>
{
    public Guid Id { get; set; }
}