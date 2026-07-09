using MediatR;
using LokynexHealth.Application.Features.Icu.DTOs;

namespace LokynexHealth.Application.Features.Icu.Queries.GetVitalsByIcuAdmission;

public class GetVitalsByIcuAdmissionQuery : IRequest<List<IcuVitalDto>>
{
    public Guid IcuAdmissionId { get; set; }
}