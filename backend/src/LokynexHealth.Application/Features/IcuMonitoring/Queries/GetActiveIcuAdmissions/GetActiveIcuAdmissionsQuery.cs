using MediatR;
using LokynexHealth.Application.Features.IcuMonitoring.DTOs;

namespace LokynexHealth.Application.Features.IcuMonitoring.Queries.GetActiveIcuAdmissions;

public class GetActiveIcuAdmissionsQuery : IRequest<List<IcuAdmissionSummaryDto>>
{
}