using MediatR;
using LokynexHealth.Application.Features.Tenants.DTOs;

namespace LokynexHealth.Application.Features.Tenants.Queries.GetAllTenants;

public class GetAllTenantsQuery : IRequest<List<TenantDto>>
{
}
