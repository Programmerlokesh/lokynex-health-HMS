using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

[ApiController]
[Route("api/doctor-opd")]
public class DoctorOpdController : ModuleDataControllerBase
{
    public DoctorOpdController(IGenericTenantDataService dataService)
        : base(dataService)
    {
    }

    protected override string ModuleKey => "doctor-opd";
}