using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateTenantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var normalizedSubdomain = request.Subdomain.Trim().ToLowerInvariant();

        var subdomainExists = await _context.Tenants
            .AnyAsync(t => t.Subdomain == normalizedSubdomain, cancellationToken);

        if (subdomainExists)
        {
            throw new InvalidOperationException($"Subdomain '{normalizedSubdomain}' is already taken.");
        }

        var tenant = new Tenant
        {
            HospitalName = request.HospitalName,
            Subdomain = normalizedSubdomain,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            Address = request.Address,
            Status = TenantStatus.PendingActivation,
            SubscriptionStartDate = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        return tenant.Id;
    }
}
