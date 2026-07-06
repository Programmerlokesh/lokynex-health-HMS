using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/blood-bank")]
public class BloodBankController : ModuleDataControllerBase
{
    public BloodBankController(
        IModuleSchemaService moduleSchemaService,
        IGenericTenantDataService dataService)
        : base(moduleSchemaService, dataService)
    {
    }

    protected override string ModuleKey => "blood-bank";
}