using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/ward-bed-management")]
public class WardBedManagementController : ModuleDataControllerBase
{
    public WardBedManagementController(IGenericTenantDataService dataService)
        : base(dataService)
    {
    }

    protected override string ModuleKey => "ward-bed-management";
}