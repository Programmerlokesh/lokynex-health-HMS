using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Platform.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Infrastructure.Persistence.Platform;

public class PlatformDbContext : DbContext, IPlatformDbContext
{
    public PlatformDbContext(DbContextOptions<PlatformDbContext> options)
        : base(options)
    {
    }

    public DbSet<PlatformTenant> Tenants => Set<PlatformTenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("superadmin");

        modelBuilder.Entity<PlatformTenant>(entity =>
        {
            entity.ToTable("tenants");

            entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Subdomain).IsRequired().HasMaxLength(100);
            entity.Property(t => t.DbSchemaName).IsRequired().HasMaxLength(100);

            entity.HasIndex(t => t.Subdomain).IsUnique();
            entity.HasIndex(t => t.DbSchemaName).IsUnique();
        });
    }
}
