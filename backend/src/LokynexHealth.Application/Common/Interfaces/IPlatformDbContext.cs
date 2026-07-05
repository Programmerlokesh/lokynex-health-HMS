using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Platform.Entities;

namespace LokynexHealth.Application.Common.Interfaces;

public interface IPlatformDbContext
{
    DbSet<PlatformTenant> Tenants { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
