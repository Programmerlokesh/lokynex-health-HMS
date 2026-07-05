using MediatR;
using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Platform.Entities;
using LokynexHealth.Domain.Platform.Enums;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Application.Features.PlatformTenants.Commands.CreatePlatformTenant;

public class CreatePlatformTenantCommandHandler : IRequestHandler<CreatePlatformTenantCommand, Guid>
{
    private readonly IPlatformDbContext _platformContext;
    private readonly ITenantProvisioningService _provisioningService;

    public CreatePlatformTenantCommandHandler(
        IPlatformDbContext platformContext,
        ITenantProvisioningService provisioningService)
    {
        _platformContext = platformContext;
        _provisioningService = provisioningService;
    }

    public async Task<Guid> Handle(CreatePlatformTenantCommand request, CancellationToken cancellationToken)
    {
        var normalizedSubdomain = request.Subdomain.Trim().ToLowerInvariant();

        var subdomainExists = await _platformContext.Tenants
            .AnyAsync(t => t.Subdomain == normalizedSubdomain, cancellationToken);

        if (subdomainExists)
        {
            throw new InvalidOperationException($"Subdomain '{normalizedSubdomain}' is already taken.");
        }

        var tenantId = Guid.NewGuid();
        var schemaName = $"hms_{tenantId.ToString("N")[..12]}";

        var tenant = new PlatformTenant
        {
            Id = tenantId,
            Name = request.Name,
            FacilityType = request.FacilityType,
            RegistrationNumber = request.RegistrationNumber,
            Gstin = request.Gstin,
            Address = request.Address,
            City = request.City,
            State = request.State,
            PinCode = request.PinCode,
            Phone = request.Phone,
            Email = request.Email,
            Subdomain = normalizedSubdomain,
            DbSchemaName = schemaName,
            Status = TenantPlatformStatus.Trial
        };

        _platformContext.Tenants.Add(tenant);
        await _platformContext.SaveChangesAsync(cancellationToken);

        await _provisioningService.ProvisionTenantSchemaAsync(schemaName, cancellationToken);

        return tenant.Id;
    }
}
