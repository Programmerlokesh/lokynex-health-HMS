using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/reports-nabh")]
public class ReportsNabhController : ModuleDataControllerBase
{
    public ReportsNabhController(IGenericTenantDataService dataService)
        : base(dataService)
    {
    }

    protected override string ModuleKey => "reports-nabh";
}