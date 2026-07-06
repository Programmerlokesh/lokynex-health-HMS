using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Npgsql;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Infrastructure.Persistence;

namespace LokynexHealth.Infrastructure.Services;

public class TenantProvisioningService : ITenantProvisioningService
{
    private readonly string _connectionString;

    public TenantProvisioningService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    }

    public async Task ProvisionTenantSchemaAsync(string schemaName, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var createSchemaCommand = new NpgsqlCommand(
            $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\";", connection);
        await createSchemaCommand.ExecuteNonQueryAsync(cancellationToken);

        // Generate the CREATE TABLE script directly from the current model
        // (which is schema-aware via ITenantContext + TenantModelCacheKeyFactory),
        // then execute it explicitly. This bypasses EnsureCreated's
        // "does the database exist" check, which is a no-op here since
        // the physical database (lokynex_health) already exists.
        var options = new DbContextOptionsBuilder<LokynexHealthDbContext>()
            .UseNpgsql(_connectionString)
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            .ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>()
            .Options;

        var fixedTenantContext = new ProvisioningTenantContext(schemaName);

        await using var context = new LokynexHealthDbContext(options, fixedTenantContext);

        var createScript = context.Database.GenerateCreateScript();

        await using var createTablesCommand = new NpgsqlCommand(createScript, connection);
        await createTablesCommand.ExecuteNonQueryAsync(cancellationToken);
    }

    private class ProvisioningTenantContext : ITenantContext
    {
        public string? SchemaName { get; }
        public Guid? TenantId => null;

        public ProvisioningTenantContext(string schemaName)
        {
            SchemaName = schemaName;
        }

        public void SetTenant(Guid tenantId, string schemaName)
        {
        }
    }
}
