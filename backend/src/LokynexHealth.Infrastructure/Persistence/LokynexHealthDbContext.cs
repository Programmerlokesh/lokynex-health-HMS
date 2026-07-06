using System.Text;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Domain.Common;
using LokynexHealth.Domain.Entities;
using LokynexHealth.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LokynexHealth.Infrastructure.Persistence;

public class LokynexHealthDbContext : DbContext, IApplicationDbContext
{
    private readonly ITenantContext _tenantContext;

    public string SchemaName => _tenantContext.SchemaName ?? "hms";

    public LokynexHealthDbContext(
        DbContextOptions<LokynexHealthDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientInsurance> PatientInsurance => Set<PatientInsurance>();
    public DbSet<PatientConsent> PatientConsents => Set<PatientConsent>();
    public DbSet<PatientDocument> PatientDocuments => Set<PatientDocument>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<AiInteractionLog> AiInteractionLog => Set<AiInteractionLog>();
    public DbSet<OpdTokenQueue> OpdTokenQueue => Set<OpdTokenQueue>();
    public DbSet<OpdVisit> OpdVisits => Set<OpdVisit>();
    public DbSet<OpdDiagnosis> OpdDiagnoses => Set<OpdDiagnosis>();
    public DbSet<OpdSoapNote> OpdSoapNotes => Set<OpdSoapNote>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<OpdInvestigationOrder> OpdInvestigationOrders => Set<OpdInvestigationOrder>();
    public DbSet<BillingRateMaster> BillingRateMasters => Set<BillingRateMaster>();
    public DbSet<BillingInvoice> BillingInvoices => Set<BillingInvoice>();
    public DbSet<BillingInvoiceItem> BillingInvoiceItems => Set<BillingInvoiceItem>();
    public DbSet<BillingRoomCharge> BillingRoomCharges => Set<BillingRoomCharge>();
    public DbSet<BillingPayment> BillingPayments => Set<BillingPayment>();
    public DbSet<BillingClaim> BillingClaims => Set<BillingClaim>();

    public DbSet<LabTestCatalog> LabTestCatalog => Set<LabTestCatalog>();
    public DbSet<LabOrder> LabOrders => Set<LabOrder>();
    public DbSet<LabOrderTest> LabOrderTests => Set<LabOrderTest>();
    public DbSet<LabSample> LabSamples => Set<LabSample>();
    public DbSet<LabResult> LabResults => Set<LabResult>();
    public DbSet<LabCriticalAlert> LabCriticalAlerts => Set<LabCriticalAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaName);

        ConfigurePatients(modelBuilder);
        ConfigureDoctorOpd(modelBuilder);
        ConfigureBilling(modelBuilder);
        ConfigureLaboratory(modelBuilder);

        UseSnakeCaseNames(modelBuilder);
    }

    private static void ConfigurePatients(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: true, hasUpdatedAt: true);
            entity.ToTable("patients");
            entity.Ignore(p => p.MedicalRecordNumber);
            entity.Ignore(p => p.PhoneNumber);

            entity.Property(p => p.Uhid).IsRequired().HasMaxLength(30);
            entity.Property(p => p.AbhaId).HasMaxLength(20);
            entity.Property(p => p.FullName).IsRequired().HasMaxLength(150);
            entity.Property(p => p.DateOfBirth).HasColumnType("date");
            entity.Property(p => p.Gender).HasEnumConversion();
            entity.Property(p => p.AadhaarNumberEnc).HasColumnType("bytea");
            entity.Property(p => p.AadhaarLast4).HasMaxLength(4);
            entity.Property(p => p.PanNumber).HasMaxLength(10);
            entity.Property(p => p.Mobile).IsRequired().HasMaxLength(15);
            entity.Property(p => p.AltMobile).HasMaxLength(15);
            entity.Property(p => p.Email).HasMaxLength(200);
            entity.Property(p => p.City).HasMaxLength(100);
            entity.Property(p => p.District).HasMaxLength(100);
            entity.Property(p => p.State).HasMaxLength(100);
            entity.Property(p => p.PinCode).HasMaxLength(6);
            entity.Property(p => p.PreferredLanguage).HasMaxLength(30);
            entity.Property(p => p.BloodGroup).HasMaxLength(5);
            entity.Property(p => p.Status).HasEnumConversion();

            entity.HasIndex(p => p.Uhid).IsUnique();
            entity.HasIndex(p => p.Mobile);
            entity.HasIndex(p => p.AbhaId);
            entity.HasQueryFilter(p => p.Status == RecordStatus.Active);
        });

        modelBuilder.Entity<PatientInsurance>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: true, hasUpdatedAt: false);
            entity.ToTable("patient_insurance");
            entity.Property(i => i.InsuranceType).HasEnumConversion();
            entity.Property(i => i.SchemeNumber).HasMaxLength(100);
            entity.Property(i => i.TpaName).HasMaxLength(150);
            entity.Property(i => i.ValidFrom).HasColumnType("date");
            entity.Property(i => i.ValidTo).HasColumnType("date");
            entity.HasIndex(i => i.PatientId);
            entity.HasOne<Patient>().WithMany().HasForeignKey(i => i.PatientId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PatientConsent>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("patient_consents");
            entity.Property(c => c.ConsentType).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Status).HasEnumConversion();
            entity.Property(c => c.CapturedVia).HasMaxLength(50);
            entity.Property(c => c.IpAddress).HasColumnType("inet");
            entity.HasIndex(c => new { c.PatientId, c.ConsentType });
            entity.HasOne<Patient>().WithMany().HasForeignKey(c => c.PatientId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PatientDocument>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("patient_documents");
            entity.Property(d => d.DocumentType).IsRequired().HasMaxLength(30);
            entity.Property(d => d.FileUrl).IsRequired();
            entity.Property(d => d.OcrExtractedJson).HasColumnType("jsonb");
            entity.Property(d => d.OcrConfidence).HasColumnType("numeric(5,4)");
            entity.HasOne<Patient>().WithMany().HasForeignKey(d => d.PatientId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureDoctorOpd(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiInteractionLog>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: true, hasUpdatedAt: false);
            entity.ToTable("ai_interaction_log");
            entity.Property(a => a.ModuleName).IsRequired().HasMaxLength(50);
            entity.Property(a => a.FeatureName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.SourceRecordTable).HasMaxLength(100);
            entity.Property(a => a.InputPayload).HasColumnType("jsonb");
            entity.Property(a => a.AiSuggestion).HasColumnType("jsonb");
            entity.Property(a => a.ConfidenceScore).HasColumnType("numeric(5,4)");
            entity.Property(a => a.ModelName).HasMaxLength(100);
            entity.Property(a => a.ModelVersion).HasMaxLength(50);
            entity.Property(a => a.ReviewStatus).HasEnumConversion();
            entity.HasIndex(a => new { a.ModuleName, a.FeatureName });
            entity.HasIndex(a => new { a.SourceRecordTable, a.SourceRecordId });
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: true, hasUpdatedAt: false);
            entity.ToTable("doctors");
            entity.Ignore(d => d.RegistrationNumber);
            entity.Ignore(d => d.IsAvailable);

            entity.Property(d => d.FullName).IsRequired().HasMaxLength(150);
            entity.Property(d => d.NmcRegistrationNo).IsRequired().HasMaxLength(50);
            entity.Property(d => d.Specialization).HasMaxLength(100);
            entity.HasIndex(d => d.NmcRegistrationNo).IsUnique();
        });

        modelBuilder.Entity<OpdTokenQueue>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("opd_token_queue");
            entity.Property(q => q.TokenNumber).IsRequired().HasMaxLength(20);
            entity.Property(q => q.QueueDate).HasColumnType("date");
            entity.Property(q => q.Status).IsRequired().HasMaxLength(20);
            entity.HasIndex(q => new { q.QueueDate, q.DoctorId });
            entity.HasIndex(q => new { q.DoctorId, q.QueueDate, q.TokenNumber }).IsUnique();
            entity.HasOne<Patient>().WithMany().HasForeignKey(q => q.PatientId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Doctor>().WithMany().HasForeignKey(q => q.DoctorId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OpdVisit>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("opd_visits");
            entity.Property(v => v.VisitNumber).IsRequired().HasMaxLength(30);
            entity.Property(v => v.SchemeTag).HasMaxLength(20);
            entity.Property(v => v.Status).HasEnumConversion();
            entity.HasIndex(v => v.VisitNumber).IsUnique();
            entity.HasIndex(v => v.PatientId);
            entity.HasIndex(v => new { v.DoctorId, v.VisitStartedAt });
            entity.HasOne<Patient>().WithMany().HasForeignKey(v => v.PatientId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Doctor>().WithMany().HasForeignKey(v => v.DoctorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<OpdTokenQueue>().WithMany().HasForeignKey(v => v.TokenId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OpdDiagnosis>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: true, hasUpdatedAt: false);
            entity.ToTable("opd_diagnoses");
            entity.Property(d => d.Icd10Code).IsRequired().HasMaxLength(10);
            entity.HasOne<OpdVisit>().WithMany().HasForeignKey(d => d.VisitId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<AiInteractionLog>().WithMany().HasForeignKey(d => d.AiLogId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OpdSoapNote>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: true, hasUpdatedAt: false);
            entity.ToTable("opd_soap_notes");
            entity.Property(n => n.SourceLanguage).HasMaxLength(10);
            entity.HasOne<OpdVisit>().WithMany().HasForeignKey(n => n.VisitId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<AiInteractionLog>().WithMany().HasForeignKey(n => n.AiLogId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("prescriptions");
            entity.Property(p => p.PrescriptionNumber).IsRequired().HasMaxLength(30);
            entity.HasIndex(p => p.PrescriptionNumber).IsUnique();
            entity.HasOne<OpdVisit>().WithMany().HasForeignKey(p => p.VisitId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Patient>().WithMany().HasForeignKey(p => p.PatientId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Doctor>().WithMany().HasForeignKey(p => p.DoctorId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PrescriptionItem>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("prescription_items");
            entity.Property(i => i.DrugName).IsRequired().HasMaxLength(200);
            entity.Property(i => i.RxnormCode).HasMaxLength(30);
            entity.Property(i => i.Dosage).HasMaxLength(50);
            entity.Property(i => i.Frequency).HasMaxLength(50);
            entity.Property(i => i.ScheduleFlag).HasEnumConversion();
            entity.HasIndex(i => i.PrescriptionId);
            entity.HasOne<Prescription>().WithMany().HasForeignKey(i => i.PrescriptionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<AiInteractionLog>().WithMany().HasForeignKey(i => i.AiLogId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OpdInvestigationOrder>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("opd_investigation_orders");
            entity.Property(o => o.OrderType).IsRequired().HasMaxLength(20);
            entity.Property(o => o.TestOrStudyName).IsRequired().HasMaxLength(200);
            entity.Property(o => o.Priority).HasEnumConversion();
            entity.HasOne<OpdVisit>().WithMany().HasForeignKey(o => o.VisitId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureBilling(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BillingRateMaster>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("billing_rate_masters");
            entity.Property(r => r.SchemeName).IsRequired().HasMaxLength(50);
            entity.Property(r => r.PackageCode).HasMaxLength(30);
            entity.Property(r => r.Rate).HasColumnType("numeric(10,2)");
            entity.Property(r => r.EffectiveFrom).HasColumnType("date");
            entity.Property(r => r.EffectiveTo).HasColumnType("date");
        });

        modelBuilder.Entity<BillingInvoice>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: true, hasUpdatedAt: false);
            entity.ToTable("billing_invoices");
            entity.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(30);
            entity.Property(i => i.InvoiceDate).HasColumnType("date");
            entity.Property(i => i.Subtotal).HasColumnType("numeric(12,2)");
            entity.Property(i => i.DiscountAmount).HasColumnType("numeric(12,2)");
            entity.Property(i => i.CgstAmount).HasColumnType("numeric(12,2)");
            entity.Property(i => i.SgstAmount).HasColumnType("numeric(12,2)");
            entity.Property(i => i.IgstAmount).HasColumnType("numeric(12,2)");
            entity.Property(i => i.TotalAmount).HasColumnType("numeric(12,2)");
            entity.Property(i => i.PaymentStatus).HasEnumConversion();
            entity.Property(i => i.EwayBillNumber).HasMaxLength(50);
            entity.HasIndex(i => i.InvoiceNumber).IsUnique();
            entity.HasIndex(i => i.PatientId);
            entity.HasOne<Patient>().WithMany().HasForeignKey(i => i.PatientId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BillingInvoiceItem>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("billing_invoice_items");
            entity.Property(i => i.Description).IsRequired();
            entity.Property(i => i.SourceModule).IsRequired().HasMaxLength(30);
            entity.Property(i => i.HsnSacCode).HasMaxLength(15);
            entity.Property(i => i.Quantity).HasColumnType("numeric(10,2)");
            entity.Property(i => i.UnitPrice).HasColumnType("numeric(10,2)");
            entity.Property(i => i.GstRatePct).HasColumnType("numeric(4,2)");
            entity.Property(i => i.LineTotal).HasColumnType("numeric(12,2)");
            entity.HasOne<BillingInvoice>().WithMany().HasForeignKey(i => i.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BillingRoomCharge>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("billing_room_charges");
            entity.Property(c => c.DailyRate).HasColumnType("numeric(10,2)");
            entity.Property(c => c.ComputedAmount).HasColumnType("numeric(12,2)");
            entity.HasOne<BillingInvoiceItem>().WithMany().HasForeignKey(c => c.InvoiceItemId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<BillingPayment>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("billing_payments");
            entity.Property(p => p.Amount).HasColumnType("numeric(12,2)");
            entity.Property(p => p.Method).HasEnumConversion();
            entity.Property(p => p.ReferenceNumber).HasMaxLength(100);
            entity.HasOne<BillingInvoice>().WithMany().HasForeignKey(p => p.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BillingClaim>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("billing_claims");
            entity.Property(c => c.ClaimType).HasEnumConversion();
            entity.Property(c => c.ClaimNumber).HasMaxLength(50);
            entity.Property(c => c.ClaimedAmount).HasColumnType("numeric(12,2)");
            entity.Property(c => c.ApprovedAmount).HasColumnType("numeric(12,2)");
            entity.Property(c => c.Status).HasEnumConversion();
            entity.HasIndex(c => c.InvoiceId);
            entity.HasOne<BillingInvoice>().WithMany().HasForeignKey(c => c.InvoiceId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<PatientInsurance>().WithMany().HasForeignKey(c => c.PatientInsuranceId).OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureLaboratory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LabTestCatalog>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("lab_test_catalog");
            entity.Property(t => t.TestCode).IsRequired().HasMaxLength(30);
            entity.Property(t => t.TestName).IsRequired().HasMaxLength(200);
            entity.Property(t => t.LoincCode).HasMaxLength(20);
            entity.Property(t => t.SpecimenType).HasMaxLength(50);
            entity.Property(t => t.NablPanel).HasMaxLength(100);
            entity.Property(t => t.StandardPrice).HasColumnType("numeric(10,2)");
            entity.Property(t => t.CghsPrice).HasColumnType("numeric(10,2)");
            entity.Property(t => t.TatHoursStd).HasColumnType("numeric(5,2)");
            entity.HasIndex(t => t.TestCode).IsUnique();
        });

        modelBuilder.Entity<LabOrder>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("lab_orders");
            entity.Property(o => o.OrderNumber).IsRequired().HasMaxLength(30);
            entity.Property(o => o.Status).HasEnumConversion();
            entity.Property(o => o.Priority).HasEnumConversion();
            entity.Property(o => o.SchemeTag).HasMaxLength(20);
            entity.HasIndex(o => o.OrderNumber).IsUnique();
            entity.HasIndex(o => o.PatientId);
            entity.HasIndex(o => o.Status);
            entity.HasOne<Patient>().WithMany().HasForeignKey(o => o.PatientId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Doctor>().WithMany().HasForeignKey(o => o.OrderingDoctorId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<AiInteractionLog>().WithMany().HasForeignKey(o => o.AiLogId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<LabOrderTest>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("lab_order_tests");
            entity.Property(t => t.PriceApplied).HasColumnType("numeric(10,2)");
            entity.HasOne<LabOrder>().WithMany().HasForeignKey(t => t.OrderId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<LabTestCatalog>().WithMany().HasForeignKey(t => t.TestId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LabSample>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("lab_samples");
            entity.Property(s => s.SampleBarcode).IsRequired().HasMaxLength(50);
            entity.HasIndex(s => s.SampleBarcode).IsUnique();
            entity.HasOne<LabOrder>().WithMany().HasForeignKey(s => s.OrderId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LabResult>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: true, hasUpdatedAt: false);
            entity.ToTable("lab_results");
            entity.Property(r => r.ParameterName).IsRequired().HasMaxLength(150);
            entity.Property(r => r.ResultValue).HasMaxLength(100);
            entity.Property(r => r.Unit).HasMaxLength(30);
            entity.Property(r => r.ReferenceRange).HasMaxLength(100);
            entity.Property(r => r.Source).HasMaxLength(20);
            entity.HasIndex(r => r.OrderTestId);
            entity.HasOne<LabOrderTest>().WithMany().HasForeignKey(r => r.OrderTestId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<LabSample>().WithMany().HasForeignKey(r => r.SampleId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<LabCriticalAlert>(entity =>
        {
            ConfigureBase(entity, hasCreatedAt: false, hasUpdatedAt: false);
            entity.ToTable("lab_critical_alerts");
            entity.Property(a => a.NotifiedVia).HasMaxLength(20);
            entity.HasOne<LabResult>().WithMany().HasForeignKey(a => a.ResultId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Doctor>().WithMany().HasForeignKey(a => a.NotifiedDoctorId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<AiInteractionLog>().WithMany().HasForeignKey(a => a.AiLogId).OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureBase<TEntity>(
        EntityTypeBuilder<TEntity> entity,
        bool hasCreatedAt,
        bool hasUpdatedAt)
        where TEntity : BaseEntity
    {
        entity.Ignore(e => e.IsDeleted);

        if (!hasCreatedAt)
        {
            entity.Ignore(e => e.CreatedAt);
        }

        if (!hasUpdatedAt)
        {
            entity.Ignore(e => e.UpdatedAt);
        }
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

    private static string ToPostgresEnum<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        return ToSnakeCase(value.ToString());
    }

    private static TEnum FromPostgresEnum<TEnum>(string value)
        where TEnum : struct, Enum
    {
        var normalized = value.Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase);
        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            if (string.Equals(enumValue.ToString(), normalized, StringComparison.OrdinalIgnoreCase))
            {
                return enumValue;
            }
        }

        throw new InvalidOperationException($"Cannot map '{value}' to {typeof(TEnum).Name}.");
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var builder = new StringBuilder(value.Length + 8);

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

public static class EnumPropertyBuilderExtensions
{
    public static PropertyBuilder<TEnum> HasEnumConversion<TEnum>(this PropertyBuilder<TEnum> builder)
        where TEnum : struct, Enum
    {
        return builder.HasConversion(
            value => LokynexHealthDbContextEnumMapper.ToPostgresEnum(value),
            value => LokynexHealthDbContextEnumMapper.FromPostgresEnum<TEnum>(value));
    }
}

internal static class LokynexHealthDbContextEnumMapper
{
    public static string ToPostgresEnum<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        return ToSnakeCase(value.ToString());
    }

    public static TEnum FromPostgresEnum<TEnum>(string value)
        where TEnum : struct, Enum
    {
        var normalized = value.Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase);
        foreach (var enumValue in Enum.GetValues<TEnum>())
        {
            if (string.Equals(enumValue.ToString(), normalized, StringComparison.OrdinalIgnoreCase))
            {
                return enumValue;
            }
        }

        throw new InvalidOperationException($"Cannot map '{value}' to {typeof(TEnum).Name}.");
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var builder = new StringBuilder(value.Length + 8);

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
