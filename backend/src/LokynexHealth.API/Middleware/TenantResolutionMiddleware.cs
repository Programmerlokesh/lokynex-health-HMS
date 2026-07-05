using Microsoft.EntityFrameworkCore;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Infrastructure.Persistence.Platform;

namespace LokynexHealth.API.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        PlatformDbContext platformContext,
        ITenantContext tenantContext)
    {
        // Skip resolution for platform-level endpoints (SuperAdmin's own tenant management)
        if (context.Request.Path.StartsWithSegments("/api/PlatformTenants") ||
            context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        var subdomain = context.Request.Headers["X-Tenant-Subdomain"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(subdomain))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "X-Tenant-Subdomain header is required." });
            return;
        }

        var tenant = await platformContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain.ToLowerInvariant());

        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { error = $"No tenant found for subdomain '{subdomain}'." });
            return;
        }

        tenantContext.SetTenant(tenant.Id, tenant.DbSchemaName);

        await _next(context);
    }
}
