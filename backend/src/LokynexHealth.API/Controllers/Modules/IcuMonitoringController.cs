using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/icu-monitoring")]
public class IcuMonitoringController : ModuleDataControllerBase
{
    public IcuMonitoringController(
        IModuleSchemaService moduleSchemaService,
        IGenericTenantDataService dataService)
        : base(moduleSchemaService, dataService)
    {
    }

    protected override string ModuleKey => "icu-monitoring";
}