using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/patient-portal-telemedicine")]
public class PatientPortalTelemedicineController : ModuleDataControllerBase
{
    public PatientPortalTelemedicineController(
        IModuleSchemaService moduleSchemaService,
        IGenericTenantDataService dataService)
        : base(moduleSchemaService, dataService)
    {
    }

    protected override string ModuleKey => "patient-portal-telemedicine";
}