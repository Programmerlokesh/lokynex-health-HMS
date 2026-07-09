using MediatR;
using LokynexHealth.Application.Features.Icu.DTOs;

namespace LokynexHealth.Application.Features.Icu.Queries.GetIoChartsByIcuAdmission;

public class GetIoChartsByIcuAdmissionQuery : IRequest<List<IcuIoChartDto>>
{
    public Guid IcuAdmissionId { get; set; }
}