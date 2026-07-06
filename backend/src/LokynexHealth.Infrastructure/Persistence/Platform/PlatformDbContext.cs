using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Platform.Entities;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Infrastructure.Persistence;

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
            entity.Property(t => t.FacilityType).HasEnumConversion();
            entity.Property(t => t.RegistrationNumber).HasMaxLength(100);
            entity.Property(t => t.Gstin).HasMaxLength(15);
            entity.Property(t => t.City).HasMaxLength(100);
            entity.Property(t => t.State).HasMaxLength(100);
            entity.Property(t => t.PinCode).HasMaxLength(6);
            entity.Property(t => t.Phone).HasMaxLength(15);
            entity.Property(t => t.Email).HasMaxLength(200);
            entity.Property(t => t.Website).HasMaxLength(200);
            entity.Property(t => t.Subdomain).IsRequired().HasMaxLength(100);
            entity.Property(t => t.DbSchemaName).IsRequired().HasMaxLength(100);
            entity.Property(t => t.RlsTag).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Status).HasEnumConversion();

            entity.HasIndex(t => t.Subdomain).IsUnique();
            entity.HasIndex(t => t.DbSchemaName).IsUnique();
            entity.HasIndex(t => t.RlsTag).IsUnique();
            entity.HasIndex(t => t.ParentTenantId);
        });

        modelBuilder.ConfigureDocsSchemaTables("superadmin");
        UseSnakeCaseNames(modelBuilder);
    }

    private static void UseSnakeCaseNames(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var builder = new System.Text.StringBuilder(value.Length + 8);

        for (var i = 0; i < value.Length; i++)
        {
            var current = value[i];

            if (char.IsUpper(current))
            {
                if (i > 0 && (char.IsLower(value[i - 1]) || char.IsDigit(value[i - 1])))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(current));
                continue;
            }

            builder.Append(char.ToLowerInvariant(current));
        }

        return builder.ToString();
    }
}
