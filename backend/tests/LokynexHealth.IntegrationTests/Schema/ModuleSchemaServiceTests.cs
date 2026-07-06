using LokynexHealth.Infrastructure.Services;

namespace LokynexHealth.IntegrationTests.Schema;

public class ModuleSchemaServiceTests
{
    [Fact]
    public void GetModules_ReturnsDocsModulesInInstallOrder()
    {
        var service = new ModuleSchemaService();

        var modules = service.GetModules();

        Assert.Collection(
            modules,
            module => Assert.Equal("shared", module.Key),
            module => Assert.Equal("platform-superadmin", module.Key),
            module => Assert.Equal("patient-registration", module.Key),
            module => Assert.Equal("doctor-opd", module.Key),
            module => Assert.Equal("laboratory", module.Key),
            module => Assert.Equal("pharmacy-pos", module.Key),
            module => Assert.Equal("ward-bed-management", module.Key),
            module => Assert.Equal("icu-monitoring", module.Key),
            module => Assert.Equal("emergency-er", module.Key),
            module => Assert.Equal("billing-finance", module.Key),
            module => Assert.Equal("reports-nabh", module.Key),
            module => Assert.Equal("ot-management", module.Key),
            module => Assert.Equal("blood-bank", module.Key),
            module => Assert.Equal("radiology-pacs", module.Key),
            module => Assert.Equal("hr-payroll", module.Key),
            module => Assert.Equal("patient-portal-telemedicine", module.Key));
    }

    [Theory]
    [InlineData("patient-registration", "patients")]
    [InlineData("doctor-opd", "opd_visits")]
    [InlineData("laboratory", "lab_orders")]
    [InlineData("pharmacy-pos", "pharmacy_sales")]
    [InlineData("ward-bed-management", "admissions")]
    [InlineData("icu-monitoring", "icu_vitals")]
    [InlineData("emergency-er", "er_visits")]
    [InlineData("billing-finance", "billing_invoices")]
    [InlineData("reports-nabh", "report_definitions")]
    [InlineData("ot-management", "ot_bookings")]
    [InlineData("blood-bank", "blood_units")]
    [InlineData("radiology-pacs", "radiology_orders")]
    [InlineData("hr-payroll", "employees")]
    [InlineData("patient-portal-telemedicine", "portal_accounts")]
    public void TableBelongsToModule_ReturnsTrueForOwningModule(string moduleKey, string tableName)
    {
        var service = new ModuleSchemaService();

        Assert.True(service.TableBelongsToModule(moduleKey, tableName));
    }

    [Fact]
    public void TableBelongsToModule_ReturnsFalseForTableFromDifferentModule()
    {
        var service = new ModuleSchemaService();

        Assert.False(service.TableBelongsToModule("laboratory", "pharmacy_sales"));
    }
}
