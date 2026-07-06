using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Npgsql;
using LokynexHealth.Application.Common.Interfaces;

namespace LokynexHealth.Infrastructure.Services;

public class TenantProvisioningService : ITenantProvisioningService
{
    private static readonly Regex SafeSchemaName = new("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

    private static readonly string[] TenantSchemaScripts =
    [
        "00_extensions_common.sql",
        "02_patient_registration.sql",
        "03_doctor_opd.sql",
        "04_laboratory.sql",
        "05_pharmacy_pos.sql",
        "06_ward_bed_management.sql",
        "07_icu_monitoring.sql",
        "08_emergency_er.sql",
        "09_billing_finance.sql",
        "10_reports_nabh.sql",
        "11_ot_management.sql",
        "12_blood_bank.sql",
        "13_radiology_pacs.sql",
        "14_hr_payroll.sql",
        "15_patient_portal_telemedicine.sql"
    ];

    private readonly string _connectionString;

    public TenantProvisioningService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    }

    public async Task ProvisionTenantSchemaAsync(string schemaName, CancellationToken cancellationToken)
    {
        if (!SafeSchemaName.IsMatch(schemaName))
        {
            throw new ArgumentException($"Invalid tenant schema name '{schemaName}'.", nameof(schemaName));
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var createSchemaCommand = new NpgsqlCommand(
            $"CREATE SCHEMA IF NOT EXISTS {QuoteIdentifier(schemaName)};", connection);
        await createSchemaCommand.ExecuteNonQueryAsync(cancellationToken);

        if (await TenantAlreadyProvisionedAsync(connection, schemaName, cancellationToken))
        {
            return;
        }

        foreach (var scriptName in TenantSchemaScripts)
        {
            var sql = await ReadEmbeddedSqlAsync(scriptName, cancellationToken);
            var tenantSql = RewriteSchemaPackSql(sql, schemaName);

            await using var command = new NpgsqlCommand(tenantSql, connection)
            {
                CommandTimeout = 0
            };

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private static async Task<bool> TenantAlreadyProvisionedAsync(
        NpgsqlConnection connection,
        string schemaName,
        CancellationToken cancellationToken)
    {
        await using var command = new NpgsqlCommand(
            """
            SELECT EXISTS (
                SELECT 1
                FROM information_schema.tables
                WHERE table_schema = @schema_name
                  AND table_name = 'patients'
            );
            """,
            connection);

        command.Parameters.AddWithValue("schema_name", schemaName);

        return (bool)(await command.ExecuteScalarAsync(cancellationToken) ?? false);
    }

    private static async Task<string> ReadEmbeddedSqlAsync(string scriptName, CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"DbSchemaReference.{scriptName}";

        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded schema script '{resourceName}' was not found.");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    private static string RewriteSchemaPackSql(string sql, string schemaName)
    {
        var quotedSchema = QuoteIdentifier(schemaName);

        return sql
            .Replace("CREATE SCHEMA IF NOT EXISTS hms;", $"CREATE SCHEMA IF NOT EXISTS {quotedSchema};")
            .Replace("SET search_path TO hms, public;", $"SET search_path TO {quotedSchema}, public;")
            .Replace("hms.", $"{quotedSchema}.")
            .Replace(
                "CREATE EXTENSION IF NOT EXISTS vector;",
                """
                DO $$
                BEGIN
                    CREATE EXTENSION IF NOT EXISTS vector;
                EXCEPTION WHEN undefined_file THEN
                    RAISE NOTICE 'pgvector extension is not installed; skipping optional vector extension';
                END $$;
                """);
    }

    private static string QuoteIdentifier(string identifier)
    {
        if (!SafeSchemaName.IsMatch(identifier))
        {
            throw new ArgumentException($"Invalid PostgreSQL identifier '{identifier}'.", nameof(identifier));
        }

        return $"\"{identifier.Replace("\"", "\"\"")}\"";
    }
}
