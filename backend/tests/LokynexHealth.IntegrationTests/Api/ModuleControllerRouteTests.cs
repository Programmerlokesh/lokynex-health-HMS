using LokynexHealth.API.Controllers.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace LokynexHealth.IntegrationTests.Api;

public class ModuleControllerRouteTests
{
    [Theory]
    [InlineData(typeof(PatientRegistrationController), "api/patient-registration")]
    [InlineData(typeof(DoctorOpdController), "api/doctor-opd")]
    [InlineData(typeof(LaboratoryController), "api/laboratory")]
    [InlineData(typeof(PharmacyController), "api/pharmacy")]
    [InlineData(typeof(WardBedManagementController), "api/ward-bed-management")]
    [InlineData(typeof(IcuMonitoringController), "api/icu-monitoring")]
    [InlineData(typeof(EmergencyErController), "api/emergency-er")]
    [InlineData(typeof(BillingFinanceModuleController), "api/billing-finance")]
    [InlineData(typeof(ReportsNabhController), "api/reports-nabh")]
    [InlineData(typeof(OtManagementController), "api/ot-management")]
    [InlineData(typeof(BloodBankController), "api/blood-bank")]
    [InlineData(typeof(RadiologyPacsController), "api/radiology-pacs")]
    [InlineData(typeof(HrPayrollController), "api/hr-payroll")]
    [InlineData(typeof(PatientPortalTelemedicineController), "api/patient-portal-telemedicine")]
    public void ModuleController_HasExpectedRoute(Type controllerType, string expectedRoute)
    {
        var route = controllerType.GetCustomAttributes(typeof(RouteAttribute), inherit: true)
            .OfType<RouteAttribute>()
            .Single();

        Assert.Equal(expectedRoute, route.Template);
    }

    [Theory]
    [InlineData(nameof(ModuleDataControllerBase.GetModuleSchema), typeof(HttpGetAttribute), "schema")]
    [InlineData(nameof(ModuleDataControllerBase.GetTables), typeof(HttpGetAttribute), "tables")]
    [InlineData(nameof(ModuleDataControllerBase.ListRows), typeof(HttpGetAttribute), "{tableName}")]
    [InlineData(nameof(ModuleDataControllerBase.GetById), typeof(HttpGetAttribute), "{tableName}/{id:guid}")]
    [InlineData(nameof(ModuleDataControllerBase.Create), typeof(HttpPostAttribute), "{tableName}")]
    [InlineData(nameof(ModuleDataControllerBase.Update), typeof(HttpPatchAttribute), "{tableName}/{id:guid}")]
    [InlineData(nameof(ModuleDataControllerBase.Delete), typeof(HttpDeleteAttribute), "{tableName}/{id:guid}")]
    public void ModuleDataControllerBase_HasExpectedActions(string methodName, Type httpAttributeType, string expectedTemplate)
    {
        var method = typeof(ModuleDataControllerBase).GetMethod(methodName);

        Assert.NotNull(method);
        var attribute = method.GetCustomAttributes(httpAttributeType, inherit: true)
            .OfType<IRouteTemplateProvider>()
            .Single();

        Assert.Equal(expectedTemplate, attribute.Template);
    }
}