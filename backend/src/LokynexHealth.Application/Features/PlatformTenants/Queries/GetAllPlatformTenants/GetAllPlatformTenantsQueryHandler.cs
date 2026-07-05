using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.PlatformTenants.DTOs;

namespace LokynexHealth.Application.Features.PlatformTenants.Queries.GetAllPlatformTenants;

public class GetAllPlatformTenantsQueryHandler : IRequestHandler<GetAllPlatformTenantsQuery, List<PlatformTenantDto>>
{
    private readonly IPlatformDbContext _context;

    public GetAllPlatformTenantsQueryHandler(IPlatformDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlatformTenantDto>> Handle(GetAllPlatformTenantsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Tenants
            .AsNoTracking()
            .Select(t => new PlatformTenantDto
            {
                Id = t.Id,
                Name = t.Name,
                FacilityType = t.FacilityType.ToString(),
                Subdomain = t.Subdomain,
                DbSchemaName = t.DbSchemaName,
                Status = t.Status.ToString()
            })
            .ToListAsync(cancellationToken);
    }
}
