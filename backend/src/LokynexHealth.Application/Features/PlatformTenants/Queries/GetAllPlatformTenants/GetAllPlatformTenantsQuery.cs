using MediatR;
using LokynexHealth.Application.Features.PlatformTenants.DTOs;

namespace LokynexHealth.Application.Features.PlatformTenants.Queries.GetAllPlatformTenants;

public class GetAllPlatformTenantsQuery : IRequest<List<PlatformTenantDto>>
{
}
