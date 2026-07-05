using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Features.Tenants.DTOs;

namespace LokynexHealth.Application.Features.Tenants.Queries.GetAllTenants;

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, List<TenantDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllTenantsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Tenants
            .AsNoTracking()
            .Select(t => new TenantDto
            {
                Id = t.Id,
                HospitalName = t.HospitalName,
                Subdomain = t.Subdomain,
                ContactEmail = t.ContactEmail,
                ContactPhone = t.ContactPhone,
                Status = t.Status.ToString(),
                SubscriptionStartDate = t.SubscriptionStartDate
            })
            .ToListAsync(cancellationToken);
    }
}
