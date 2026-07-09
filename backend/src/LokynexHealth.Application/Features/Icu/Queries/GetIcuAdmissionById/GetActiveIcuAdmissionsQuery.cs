using MediatR;
using LokynexHealth.Application.Features.Icu.DTOs;

namespace LokynexHealth.Application.Features.Icu.Queries.GetActiveIcuAdmissions;

public class GetActiveIcuAdmissionsQuery : IRequest<List<IcuAdmissionDto>>
{
}