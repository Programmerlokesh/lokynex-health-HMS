using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.PlatformTenants.Commands.CreatePlatformTenant;
using LokynexHealth.Application.Features.PlatformTenants.Queries.GetAllPlatformTenants;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformTenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlatformTenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] CreatePlatformTenantCommand command)
    {
        var tenantId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAllTenants), new { id = tenantId }, new { id = tenantId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTenants()
    {
        var tenants = await _mediator.Send(new GetAllPlatformTenantsQuery());
        return Ok(tenants);
    }
}
