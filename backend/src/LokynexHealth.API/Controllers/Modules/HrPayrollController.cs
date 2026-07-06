using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/hr-payroll")]
public class HrPayrollController : ModuleDataControllerBase
{
    public HrPayrollController(
        IModuleSchemaService moduleSchemaService,
        IGenericTenantDataService dataService)
        : base(moduleSchemaService, dataService)
    {
    }

    protected override string ModuleKey => "hr-payroll";
}