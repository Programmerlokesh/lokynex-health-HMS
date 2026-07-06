using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/ward-bed-management")]
public class WardBedManagementController : ModuleDataControllerBase
{
    public WardBedManagementController(
        IModuleSchemaService moduleSchemaService,
        IGenericTenantDataService dataService)
        : base(moduleSchemaService, dataService)
    {
    }

    protected override string ModuleKey => "ward-bed-management";
}