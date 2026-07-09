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
    DbSet<LabTestCatalog> LabTestCatalog { get; }
    DbSet<LabOrder> LabOrders { get; }
    DbSet<LabOrderTest> LabOrderTests { get; }
    DbSet<LabSample> LabSamples { get; }
    DbSet<LabResult> LabResults { get; }
    DbSet<LabCriticalAlert> LabCriticalAlerts { get; }
    DbSet<PharmacyDrugCatalog> PharmacyDrugCatalog { get; }
    DbSet<PharmacyStockBatch> PharmacyStockBatches { get; }
    DbSet<PharmacySale> PharmacySales { get; }
    DbSet<PharmacySaleItem> PharmacySaleItems { get; }
    DbSet<Ward> Wards { get; }
    DbSet<Bed> Beds { get; }
    DbSet<Admission> Admissions { get; }
    DbSet<NursingAssessment> NursingAssessments { get; }
    DbSet<BedTransfer> BedTransfers { get; }
    DbSet<HousekeepingTask> HousekeepingTasks { get; }
    DbSet<IcuAdmission> IcuAdmissions { get; }
    DbSet<IcuVital> IcuVitals { get; }
    DbSet<IcuVentilatorRecord> IcuVentilatorRecords { get; }
    DbSet<IcuIoChart> IcuIoCharts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
