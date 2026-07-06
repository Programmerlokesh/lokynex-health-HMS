using System.Text.Json;
using LokynexHealth.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LokynexHealth.API.Controllers.Modules;

public abstract class ModuleDataControllerBase : ControllerBase
{
    private readonly IModuleSchemaService _moduleSchemaService;
    private readonly IGenericTenantDataService _dataService;

    protected ModuleDataControllerBase(
        IModuleSchemaService moduleSchemaService,
        IGenericTenantDataService dataService)
    {
        _moduleSchemaService = moduleSchemaService;
        _dataService = dataService;
    }

    protected abstract string ModuleKey { get; }

    [HttpGet("schema")]
    public IActionResult GetModuleSchema()
    {
        var module = _moduleSchemaService.GetModule(ModuleKey);
        return module is null ? NotFound(new { error = $"Module '{ModuleKey}' was not found." }) : Ok(module);
    }

    [HttpGet("tables")]
    public IActionResult GetTables()
    {
        var module = _moduleSchemaService.GetModule(ModuleKey);
        return module is null ? NotFound(new { error = $"Module '{ModuleKey}' was not found." }) : Ok(module.Tables);
    }

    [HttpGet("{tableName}")]
    public async Task<IActionResult> ListRows(
        string tableName,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateTable(tableName);
        if (validation is not null)
        {
            return validation;
        }

        return Ok(await _dataService.ListAsync(tableName, limit, offset, cancellationToken));
    }

    [HttpGet("{tableName}/{id:guid}")]
    public async Task<IActionResult> GetById(
        string tableName,
        Guid id,
        CancellationToken cancellationToken)
    {
        var validation = ValidateTable(tableName);
        if (validation is not null)
        {
            return validation;
        }

        var row = await _dataService.GetByIdAsync(tableName, id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpPost("{tableName}")]
    public async Task<IActionResult> Create(
        string tableName,
        [FromBody] JsonElement payload,
        CancellationToken cancellationToken)
    {
        var validation = ValidateTable(tableName);
        if (validation is not null)
        {
            return validation;
        }

        var id = await _dataService.CreateAsync(tableName, payload, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { tableName, id }, new { id });
    }

    [HttpPatch("{tableName}/{id:guid}")]
    public async Task<IActionResult> Update(
        string tableName,
        Guid id,
        [FromBody] JsonElement payload,
        CancellationToken cancellationToken)
    {
        var validation = ValidateTable(tableName);
        if (validation is not null)
        {
            return validation;
        }

        var updated = await _dataService.UpdateAsync(tableName, id, payload, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{tableName}/{id:guid}")]
    public async Task<IActionResult> Delete(
        string tableName,
        Guid id,
        CancellationToken cancellationToken)
    {
        var validation = ValidateTable(tableName);
        if (validation is not null)
        {
            return validation;
        }

        var deleted = await _dataService.DeleteAsync(tableName, id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private IActionResult? ValidateTable(string tableName)
    {
        var module = _moduleSchemaService.GetModule(ModuleKey);
        if (module is null)
        {
            return NotFound(new { error = $"Module '{ModuleKey}' was not found." });
        }

        if (!string.Equals(module.SchemaName, "hms", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { error = $"Module '{ModuleKey}' is not a tenant data module." });
        }

        if (!_moduleSchemaService.TableBelongsToModule(ModuleKey, tableName))
        {
            return NotFound(new { error = $"Table '{tableName}' is not part of module '{ModuleKey}'." });
        }

        return null;
    }
}