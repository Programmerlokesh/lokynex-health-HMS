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

        await using var command = new NpgsqlCommand(
            $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\";", connection);

        await command.ExecuteNonQueryAsync(cancellationToken);

        await CreateTenantSchemaTablesAsync(schemaName, cancellationToken);
    }

    private async Task CreateTenantSchemaTablesAsync(string schemaName, CancellationToken cancellationToken)
    {
        var options = new DbContextOptionsBuilder<LokynexHealthDbContext>()
            .UseNpgsql(_connectionString)
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            .ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>()
            .Options;

        var fixedTenantContext = new ProvisioningTenantContext(schemaName);

        await using var context = new LokynexHealthDbContext(options, fixedTenantContext);

        // EnsureCreated builds DDL directly from the current dynamic model
        // (respecting HasDefaultSchema at runtime), unlike Migrate() which
        // replays compiled migration code with a schema name baked in at
        // generation time.
        await context.Database.EnsureCreatedAsync(cancellationToken);
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
