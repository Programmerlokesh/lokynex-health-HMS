using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace LokynexHealth.Infrastructure.Services;

public class GenericTenantDataService : IGenericTenantDataService
{
    private static readonly Regex SafeIdentifier = new("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

    private readonly string _connectionString;
    private readonly ITenantContext _tenantContext;

    public GenericTenantDataService(IConfiguration configuration, ITenantContext tenantContext)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<TenantTableDto>> GetTablesAsync(CancellationToken cancellationToken)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(
            """
            SELECT table_name
            FROM information_schema.tables
            WHERE table_schema = @schema
              AND table_type = 'BASE TABLE'
            ORDER BY table_name;
            """,
            connection);
        command.Parameters.AddWithValue("schema", SchemaName);

        var tables = new List<TenantTableDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            tables.Add(new TenantTableDto { Name = reader.GetString(0) });
        }

        await reader.CloseAsync();

        foreach (var table in tables)
        {
            table.Columns = (await GetColumnsAsync(connection, table.Name, cancellationToken))
                .Select(c => new TenantColumnDto
                {
                    Name = c.Name,
                    DataType = c.DataType == "USER-DEFINED" ? c.UdtName : c.DataType,
                    IsNullable = c.IsNullable,
                    HasDefault = c.HasDefault
                })
                .ToList();
        }

        return tables;
    }

    public async Task<IReadOnlyList<Dictionary<string, object?>>> ListAsync(
        string tableName,
        int limit,
        int offset,
        CancellationToken cancellationToken)
    {
        ValidateIdentifier(tableName);
        limit = Math.Clamp(limit, 1, 500);
        offset = Math.Max(offset, 0);

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var columns = await GetColumnsAsync(connection, tableName, cancellationToken);
        EnsureTableExists(tableName, columns);

        var orderColumn = columns.Any(c => c.Name == "created_at") ? "created_at" : "id";
        var sql = $"""
                   SELECT *
                   FROM {QualifiedTable(tableName)}
                   ORDER BY {QuoteIdentifier(orderColumn)} DESC
                   LIMIT @limit OFFSET @offset;
                   """;

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("limit", limit);
        command.Parameters.AddWithValue("offset", offset);

        return await ReadRowsAsync(command, cancellationToken);
    }

    public async Task<Dictionary<string, object?>?> GetByIdAsync(
        string tableName,
        Guid id,
        CancellationToken cancellationToken)
    {
        ValidateIdentifier(tableName);

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var columns = await GetColumnsAsync(connection, tableName, cancellationToken);
        EnsureTableExists(tableName, columns);
        EnsureIdColumn(tableName, columns);

        var sql = $"SELECT * FROM {QualifiedTable(tableName)} WHERE id = @id;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", id);

        return (await ReadRowsAsync(command, cancellationToken)).FirstOrDefault();
    }

    public async Task<Guid> CreateAsync(string tableName, JsonElement payload, CancellationToken cancellationToken)
    {
        ValidateIdentifier(tableName);
        EnsureObjectPayload(payload);

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var columns = await GetColumnsAsync(connection, tableName, cancellationToken);
        EnsureTableExists(tableName, columns);

        var payloadColumns = MapPayloadToColumns(payload, columns, allowId: true);
        var sql = new StringBuilder();
        sql.Append($"INSERT INTO {QualifiedTable(tableName)}");

        await using var command = new NpgsqlCommand { Connection = connection };

        if (payloadColumns.Count == 0)
        {
            sql.Append(" DEFAULT VALUES");
        }
        else
        {
            sql.Append(" (");
            sql.Append(string.Join(", ", payloadColumns.Select(c => QuoteIdentifier(c.Column.Name))));
            sql.Append(") VALUES (");
            sql.Append(string.Join(", ", payloadColumns.Select((c, index) =>
            {
                var parameterName = $"p{index}";
                AddParameter(command, parameterName, c.Column, c.Value);
                return ParameterReference(parameterName, c.Column);
            })));
            sql.Append(')');
        }

        sql.Append(" RETURNING id;");
        command.CommandText = sql.ToString();

        var createdId = await command.ExecuteScalarAsync(cancellationToken);
        return createdId is Guid id
            ? id
            : throw new InvalidOperationException($"Table '{tableName}' did not return a UUID id.");
    }

    public async Task<bool> UpdateAsync(string tableName, Guid id, JsonElement payload, CancellationToken cancellationToken)
    {
        ValidateIdentifier(tableName);
        EnsureObjectPayload(payload);

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var columns = await GetColumnsAsync(connection, tableName, cancellationToken);
        EnsureTableExists(tableName, columns);
        EnsureIdColumn(tableName, columns);

        var payloadColumns = MapPayloadToColumns(payload, columns, allowId: false);
        if (payloadColumns.Count == 0)
        {
            return false;
        }

        await using var command = new NpgsqlCommand { Connection = connection };
        command.Parameters.AddWithValue("id", id);

        var assignments = payloadColumns.Select((c, index) =>
        {
            var parameterName = $"p{index}";
            AddParameter(command, parameterName, c.Column, c.Value);
            return $"{QuoteIdentifier(c.Column.Name)} = {ParameterReference(parameterName, c.Column)}";
        });

        command.CommandText = $"""
                              UPDATE {QualifiedTable(tableName)}
                              SET {string.Join(", ", assignments)}
                              WHERE id = @id;
                              """;

        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(string tableName, Guid id, CancellationToken cancellationToken)
    {
        ValidateIdentifier(tableName);

        await using var connection = await OpenConnectionAsync(cancellationToken);
        var columns = await GetColumnsAsync(connection, tableName, cancellationToken);
        EnsureTableExists(tableName, columns);
        EnsureIdColumn(tableName, columns);

        var statusColumn = columns.FirstOrDefault(c => c.Name == "status" && c.UdtName == "record_status");
        if (statusColumn is not null)
        {
            await using var softDeleteCommand = new NpgsqlCommand(
                $"""
                 UPDATE {QualifiedTable(tableName)}
                 SET status = @status::{QualifiedType(statusColumn.UdtName)}
                 WHERE id = @id;
                 """,
                connection);
            softDeleteCommand.Parameters.AddWithValue("id", id);
            softDeleteCommand.Parameters.AddWithValue("status", "inactive");

            return await softDeleteCommand.ExecuteNonQueryAsync(cancellationToken) > 0;
        }

        if (columns.Any(c => c.Name == "is_active" && c.DataType == "boolean"))
        {
            await using var deactivateCommand = new NpgsqlCommand(
                $"""
                 UPDATE {QualifiedTable(tableName)}
                 SET is_active = FALSE
                 WHERE id = @id;
                 """,
                connection);
            deactivateCommand.Parameters.AddWithValue("id", id);

            return await deactivateCommand.ExecuteNonQueryAsync(cancellationToken) > 0;
        }

        await using var deleteCommand = new NpgsqlCommand(
            $"DELETE FROM {QualifiedTable(tableName)} WHERE id = @id;",
            connection);
        deleteCommand.Parameters.AddWithValue("id", id);

        return await deleteCommand.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    private string SchemaName => _tenantContext.SchemaName
        ?? throw new InvalidOperationException("Tenant context has not been resolved.");

    private async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    private async Task<List<ColumnMetadata>> GetColumnsAsync(
        NpgsqlConnection connection,
        string tableName,
        CancellationToken cancellationToken)
    {
        await using var command = new NpgsqlCommand(
            """
            SELECT column_name, data_type, udt_name, is_nullable, column_default
            FROM information_schema.columns
            WHERE table_schema = @schema
              AND table_name = @table
            ORDER BY ordinal_position;
            """,
            connection);
        command.Parameters.AddWithValue("schema", SchemaName);
        command.Parameters.AddWithValue("table", tableName);

        var columns = new List<ColumnMetadata>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            columns.Add(new ColumnMetadata(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3) == "YES",
                !reader.IsDBNull(4)));
        }

        return columns;
    }

    private static async Task<List<Dictionary<string, object?>>> ReadRowsAsync(
        NpgsqlCommand command,
        CancellationToken cancellationToken)
    {
        var rows = new List<Dictionary<string, object?>>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = await reader.IsDBNullAsync(i, cancellationToken)
                    ? null
                    : reader.GetValue(i);
            }

            rows.Add(row);
        }

        return rows;
    }

    private List<(ColumnMetadata Column, JsonElement Value)> MapPayloadToColumns(
        JsonElement payload,
        IReadOnlyList<ColumnMetadata> columns,
        bool allowId)
    {
        var writableColumns = columns
            .Where(c => allowId || c.Name != "id")
            .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        var mapped = new List<(ColumnMetadata Column, JsonElement Value)>();

        foreach (var property in payload.EnumerateObject())
        {
            var columnName = writableColumns.ContainsKey(property.Name)
                ? property.Name
                : ToSnakeCase(property.Name);

            if (!writableColumns.TryGetValue(columnName, out var column))
            {
                throw new InvalidOperationException($"Column '{property.Name}' is not valid for table payload.");
            }

            mapped.Add((column, property.Value));
        }

        return mapped;
    }

    private static void AddParameter(NpgsqlCommand command, string name, ColumnMetadata column, JsonElement value)
    {
        var parameter = command.Parameters.AddWithValue(name, ToDbValue(column, value));

        if (value.ValueKind == JsonValueKind.Null)
        {
            parameter.Value = DBNull.Value;
        }

        if (column.DataType is "json" or "jsonb")
        {
            parameter.NpgsqlDbType = column.DataType == "jsonb" ? NpgsqlDbType.Jsonb : NpgsqlDbType.Json;
        }
    }

    private static object ToDbValue(ColumnMetadata column, JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Null)
        {
            return DBNull.Value;
        }

        if (column.DataType is "json" or "jsonb")
        {
            return value.GetRawText();
        }

        return value.ValueKind switch
        {
            JsonValueKind.String => ConvertString(column, value.GetString() ?? string.Empty),
            JsonValueKind.Number when column.DataType is "integer" => value.GetInt32(),
            JsonValueKind.Number when column.DataType is "smallint" => value.GetInt16(),
            JsonValueKind.Number when column.DataType is "bigint" => value.GetInt64(),
            JsonValueKind.Number when column.DataType is "real" => value.GetSingle(),
            JsonValueKind.Number when column.DataType is "double precision" => value.GetDouble(),
            JsonValueKind.Number => value.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Array => value.GetRawText(),
            JsonValueKind.Object => value.GetRawText(),
            _ => value.GetRawText()
        };
    }

    private static object ConvertString(ColumnMetadata column, string value)
    {
        return column.DataType switch
        {
            "uuid" => Guid.Parse(value),
            "date" => DateOnly.Parse(value),
            "timestamp with time zone" => DateTimeOffset.Parse(value),
            "timestamp without time zone" => DateTime.Parse(value),
            "boolean" => bool.Parse(value),
            "integer" => int.Parse(value),
            "smallint" => short.Parse(value),
            "bigint" => long.Parse(value),
            "numeric" => decimal.Parse(value),
            "inet" => IPAddress.Parse(value),
            _ => value
        };
    }

    private string ParameterReference(string parameterName, ColumnMetadata column)
    {
        if (column.DataType == "USER-DEFINED")
        {
            return $"@{parameterName}::{QualifiedType(column.UdtName)}";
        }

        if (column.DataType is "json" or "jsonb")
        {
            return $"@{parameterName}::{column.DataType}";
        }

        if (column.DataType == "inet")
        {
            return $"@{parameterName}::inet";
        }

        return $"@{parameterName}";
    }

    private string QualifiedTable(string tableName)
    {
        ValidateIdentifier(tableName);
        return $"{QuoteIdentifier(SchemaName)}.{QuoteIdentifier(tableName)}";
    }

    private string QualifiedType(string typeName)
    {
        ValidateIdentifier(typeName);
        return $"{QuoteIdentifier(SchemaName)}.{QuoteIdentifier(typeName)}";
    }

    private static void EnsureTableExists(string tableName, IReadOnlyList<ColumnMetadata> columns)
    {
        if (columns.Count == 0)
        {
            throw new KeyNotFoundException($"Table '{tableName}' was not found in the current tenant schema.");
        }
    }

    private static void EnsureIdColumn(string tableName, IReadOnlyList<ColumnMetadata> columns)
    {
        if (!columns.Any(c => c.Name == "id" && c.DataType == "uuid"))
        {
            throw new InvalidOperationException($"Table '{tableName}' does not expose a UUID id column.");
        }
    }

    private static void EnsureObjectPayload(JsonElement payload)
    {
        if (payload.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Request body must be a JSON object.");
        }
    }

    private static void ValidateIdentifier(string identifier)
    {
        if (!SafeIdentifier.IsMatch(identifier))
        {
            throw new ArgumentException($"Invalid identifier '{identifier}'.", nameof(identifier));
        }
    }

    private static string QuoteIdentifier(string identifier)
    {
        ValidateIdentifier(identifier);
        return $"\"{identifier.Replace("\"", "\"\"")}\"";
    }

    private static string ToSnakeCase(string value)
    {
        var builder = new StringBuilder(value.Length + 8);

        for (var i = 0; i < value.Length; i++)
        {
            var current = value[i];
            if (char.IsUpper(current))
            {
                if (i > 0 && (char.IsLower(value[i - 1]) || char.IsDigit(value[i - 1])))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(current));
                continue;
            }

            builder.Append(char.ToLowerInvariant(current));
        }

        return builder.ToString();
    }

    private sealed record ColumnMetadata(
        string Name,
        string DataType,
        string UdtName,
        bool IsNullable,
        bool HasDefault);
}
