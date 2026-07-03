using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;

namespace LokynexHealth.Infrastructure.Persistence;

public class LokynexHealthDbContext : DbContext
{
    public LokynexHealthDbContext(DbContextOptions<LokynexHealthDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enforce required fields and sensible max lengths at the DB level
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.Property(t => t.HospitalName).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Subdomain).IsRequired().HasMaxLength(50);
            entity.HasIndex(t => t.Subdomain).IsUnique();
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(p => p.FullName).IsRequired().HasMaxLength(200);
            entity.Property(p => p.MedicalRecordNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(p => new { p.TenantId, p.MedicalRecordNumber }).IsUnique();
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.Property(d => d.FullName).IsRequired().HasMaxLength(200);
            entity.Property(d => d.RegistrationNumber).IsRequired().HasMaxLength(50);
        });
    }
}