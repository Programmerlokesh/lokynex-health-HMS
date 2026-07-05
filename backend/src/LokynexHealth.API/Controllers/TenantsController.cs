using MediatR;
using Microsoft.AspNetCore.Mvc;
using LokynexHealth.Application.Features.Tenants.Commands.CreateTenant;
using LokynexHealth.Application.Features.Tenants.Queries.GetAllTenants;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command)
    {
        var tenantId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAllTenants), new { id = tenantId }, new { id = tenantId });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTenants()
    {
        var tenants = await _mediator.Send(new GetAllTenantsQuery());
        return Ok(tenants);
    }
}
