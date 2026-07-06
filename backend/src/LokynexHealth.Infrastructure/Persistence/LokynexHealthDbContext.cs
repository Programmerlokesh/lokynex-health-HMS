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

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<BillingInvoice> BillingInvoices => Set<BillingInvoice>();
    public DbSet<BillingInvoiceItem> BillingInvoiceItems => Set<BillingInvoiceItem>();
    public DbSet<BillingPayment> BillingPayments => Set<BillingPayment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(p => p.FullName).IsRequired().HasMaxLength(200);
            entity.Property(p => p.MedicalRecordNumber).IsRequired().HasMaxLength(50);
            entity.Property(p => p.DateOfBirth).HasColumnType("date");
            entity.HasIndex(p => p.MedicalRecordNumber).IsUnique();

            entity.HasQueryFilter(p => !p.IsDeleted);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.Property(d => d.FullName).IsRequired().HasMaxLength(200);
            entity.Property(d => d.RegistrationNumber).IsRequired().HasMaxLength(50);

            entity.HasQueryFilter(d => !d.IsDeleted);
        });

        modelBuilder.Entity<BillingInvoice>(entity =>
        {
            entity.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(30);
            entity.HasIndex(i => i.InvoiceNumber).IsUnique();

            entity.Property(i => i.Subtotal).HasColumnType("numeric(12,2)");
            entity.Property(i => i.DiscountAmount).HasColumnType("numeric(12,2)");
            entity.Property(i => i.CgstAmount).HasColumnType("numeric(12,2)");
            entity.Property(i => i.SgstAmount).HasColumnType("numeric(12,2)");
            entity.Property(i => i.IgstAmount).HasColumnType("numeric(12,2)");
            entity.Property(i => i.TotalAmount).HasColumnType("numeric(12,2)");

            entity.HasOne<Patient>()
                .WithMany()
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(i => !i.IsDeleted);
        });

        modelBuilder.Entity<BillingInvoiceItem>(entity =>
        {
            entity.Property(i => i.Description).IsRequired();
            entity.Property(i => i.SourceModule).IsRequired().HasMaxLength(30);
            entity.Property(i => i.Quantity).HasColumnType("numeric(10,2)");
            entity.Property(i => i.UnitPrice).HasColumnType("numeric(10,2)");
            entity.Property(i => i.GstRatePct).HasColumnType("numeric(4,2)");
            entity.Property(i => i.LineTotal).HasColumnType("numeric(12,2)");

            entity.HasOne<BillingInvoice>()
                .WithMany()
                .HasForeignKey(i => i.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(i => !i.IsDeleted);
        });

        modelBuilder.Entity<BillingPayment>(entity =>
        {
            entity.Property(p => p.Amount).HasColumnType("numeric(12,2)");

            entity.HasOne<BillingInvoice>()
                .WithMany()
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(p => !p.IsDeleted);
        });
    }
}
