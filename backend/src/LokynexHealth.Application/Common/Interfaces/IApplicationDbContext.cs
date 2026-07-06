using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;

namespace LokynexHealth.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<BillingInvoice> BillingInvoices { get; }
    DbSet<BillingInvoiceItem> BillingInvoiceItems { get; }
    DbSet<BillingPayment> BillingPayments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
