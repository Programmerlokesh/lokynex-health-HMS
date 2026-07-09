using MediatR;
using LokynexHealth.Application.Features.IcuMonitoring.DTOs;

namespace LokynexHealth.Application.Features.IcuMonitoring.Queries.GetVitalsByAdmission;

public class GetVitalsByAdmissionQuery : IRequest<List<IcuVitalDto>>
{
    public Guid IcuAdmissionId { get; set; }
}