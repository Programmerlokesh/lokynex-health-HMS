using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/blood-bank")]
public class BloodBankController : ModuleDataControllerBase
{
    public BloodBankController(IGenericTenantDataService dataService)
        : base(dataService)
    {
    }

    protected override string ModuleKey => "blood-bank";
}