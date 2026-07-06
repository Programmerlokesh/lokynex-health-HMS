using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Infrastructure.Persistence;

public class LokynexHealthDbContext : DbContext, IApplicationDbContext
{
    private readonly ITenantContext _tenantContext;

    public string SchemaName => _tenantContext.SchemaName ?? "hms_default";

    public LokynexHealthDbContext(
        DbContextOptions<LokynexHealthDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.Property(t => t.HospitalName).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Subdomain).IsRequired().HasMaxLength(50);
            entity.HasIndex(t => t.Subdomain).IsUnique();
            entity.HasQueryFilter(t => !t.IsDeleted);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(p => p.FullName).IsRequired().HasMaxLength(200);
            entity.Property(p => p.MedicalRecordNumber).IsRequired().HasMaxLength(50);
            entity.Property(p => p.DateOfBirth).HasColumnType("date");
            entity.HasIndex(p => new { p.TenantId, p.MedicalRecordNumber }).IsUnique();

            entity.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(p => !p.IsDeleted);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.Property(d => d.FullName).IsRequired().HasMaxLength(200);
            entity.Property(d => d.RegistrationNumber).IsRequired().HasMaxLength(50);

            entity.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(d => !d.IsDeleted);
        });
    }
}
