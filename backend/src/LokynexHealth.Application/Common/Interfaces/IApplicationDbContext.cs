using Microsoft.EntityFrameworkCore;
using LokynexHealth.Domain.Entities;

namespace LokynexHealth.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Patient> Patients { get; }
    DbSet<PatientInsurance> PatientInsurance { get; }
    DbSet<PatientConsent> PatientConsents { get; }
    DbSet<PatientDocument> PatientDocuments { get; }
    DbSet<Doctor> Doctors { get; }
    DbSet<AiInteractionLog> AiInteractionLog { get; }
    DbSet<OpdTokenQueue> OpdTokenQueue { get; }
    DbSet<OpdVisit> OpdVisits { get; }
    DbSet<OpdDiagnosis> OpdDiagnoses { get; }
    DbSet<OpdSoapNote> OpdSoapNotes { get; }
    DbSet<Prescription> Prescriptions { get; }
    DbSet<PrescriptionItem> PrescriptionItems { get; }
    DbSet<OpdInvestigationOrder> OpdInvestigationOrders { get; }
    DbSet<BillingRateMaster> BillingRateMasters { get; }
    DbSet<BillingInvoice> BillingInvoices { get; }
    DbSet<BillingInvoiceItem> BillingInvoiceItems { get; }
    DbSet<BillingRoomCharge> BillingRoomCharges { get; }
    DbSet<BillingPayment> BillingPayments { get; }
    DbSet<BillingClaim> BillingClaims { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
