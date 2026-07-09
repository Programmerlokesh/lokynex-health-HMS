using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/ot-management")]
public class OtManagementController : ModuleDataControllerBase
{
    public OtManagementController(IGenericTenantDataService dataService)
        : base(dataService)
    {
    }

    protected override string ModuleKey => "ot-management";
}