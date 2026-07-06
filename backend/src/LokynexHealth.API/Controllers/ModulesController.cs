using System.Text.Json;
using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers;

[ApiController]
[Route("api/modules")]
public class ModulesController : ControllerBase
{
    private readonly IModuleSchemaService _moduleSchemaService;
    private readonly IGenericTenantDataService _dataService;

    public ModulesController(
        IModuleSchemaService moduleSchemaService,
        IGenericTenantDataService dataService)
    {
        _moduleSchemaService = moduleSchemaService;
        _dataService = dataService;
    }

    [HttpGet]
    public IActionResult GetModules()
    {
        return Ok(_moduleSchemaService.GetModules());
    }

    [HttpGet("{moduleKey}")]
    public IActionResult GetModule(string moduleKey)
    {
        var module = _moduleSchemaService.GetModule(moduleKey);
        return module is null ? NotFound(new { error = $"Module '{moduleKey}' was not found." }) : Ok(module);
    }

    [HttpGet("{moduleKey}/tables")]
    public IActionResult GetModuleTables(string moduleKey)
    {
        var module = _moduleSchemaService.GetModule(moduleKey);
        return module is null ? NotFound(new { error = $"Module '{moduleKey}' was not found." }) : Ok(module.Tables);
    }

    [HttpGet("{moduleKey}/{tableName}")]
    public async Task<IActionResult> ListRows(
        string moduleKey,
        string tableName,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateTenantModuleTable(moduleKey, tableName);
        if (validation is not null)
        {
            return validation;
        }

        return Ok(await _dataService.ListAsync(tableName, limit, offset, cancellationToken));
    }

    [HttpGet("{moduleKey}/{tableName}/{id:guid}")]
    public async Task<IActionResult> GetById(
        string moduleKey,
        string tableName,
        Guid id,
        CancellationToken cancellationToken)
    {
        var validation = ValidateTenantModuleTable(moduleKey, tableName);
        if (validation is not null)
        {
            return validation;
        }

        var row = await _dataService.GetByIdAsync(tableName, id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpPost("{moduleKey}/{tableName}")]
    public async Task<IActionResult> Create(
        string moduleKey,
        string tableName,
        [FromBody] JsonElement payload,
        CancellationToken cancellationToken)
    {
        var validation = ValidateTenantModuleTable(moduleKey, tableName);
        if (validation is not null)
        {
            return validation;
        }

        var id = await _dataService.CreateAsync(tableName, payload, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { moduleKey, tableName, id }, new { id });
    }

    [HttpPatch("{moduleKey}/{tableName}/{id:guid}")]
    public async Task<IActionResult> Update(
        string moduleKey,
        string tableName,
        Guid id,
        [FromBody] JsonElement payload,
        CancellationToken cancellationToken)
    {
        var validation = ValidateTenantModuleTable(moduleKey, tableName);
        if (validation is not null)
        {
            return validation;
        }

        var updated = await _dataService.UpdateAsync(tableName, id, payload, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{moduleKey}/{tableName}/{id:guid}")]
    public async Task<IActionResult> Delete(
        string moduleKey,
        string tableName,
        Guid id,
        CancellationToken cancellationToken)
    {
        var validation = ValidateTenantModuleTable(moduleKey, tableName);
        if (validation is not null)
        {
            return validation;
        }

        var deleted = await _dataService.DeleteAsync(tableName, id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private IActionResult? ValidateTenantModuleTable(string moduleKey, string tableName)
    {
        var module = _moduleSchemaService.GetModule(moduleKey);
        if (module is null)
        {
            return NotFound(new { error = $"Module '{moduleKey}' was not found." });
        }

        if (!string.Equals(module.SchemaName, "hms", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { error = $"Module '{moduleKey}' belongs to schema '{module.SchemaName}' and is not a tenant data module." });
        }

        if (!_moduleSchemaService.TableBelongsToModule(moduleKey, tableName))
        {
            return NotFound(new { error = $"Table '{tableName}' is not part of module '{moduleKey}'." });
        }

        return null;
    }
}
