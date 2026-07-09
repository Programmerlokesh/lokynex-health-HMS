using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/emergency-er")]
public class EmergencyErController : ModuleDataControllerBase
{
    public EmergencyErController(IGenericTenantDataService dataService)
        : base(dataService)
    {
    }

    protected override string ModuleKey => "emergency-er";
}