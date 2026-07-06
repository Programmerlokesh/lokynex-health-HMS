using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace LokynexHealth.Infrastructure.Persistence;

internal static class DocsSchemaModelBuilderExtensions
{
    private static readonly Regex CreateTableRegex = new(
        @"^\s*CREATE\s+TABLE\s+(?:IF\s+NOT\s+EXISTS\s+)?(?<table>""?[\w.]+""?)\s*\(",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ColumnRegex = new(
        @"^""?(?<name>[A-Za-z_][A-Za-z0-9_]*)""?\s+(?<type>.+)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly HashSet<string> ConstraintKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "CONSTRAINT",
        "PRIMARY",
        "FOREIGN",
        "UNIQUE",
        "CHECK",
        "EXCLUDE"
    };

    public static void ConfigureDocsSchemaTables(
        this ModelBuilder modelBuilder,
        string schemaName,
        IReadOnlySet<string>? excludedTables = null)
    {
        var existingTables = modelBuilder.Model
            .GetEntityTypes()
            .Select(e => e.GetTableName())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (excludedTables is not null)
        {
            foreach (var table in excludedTables)
            {
                existingTables.Add(table);
            }
        }

        foreach (var table in ReadSchemaTables(schemaName))
        {
            if (existingTables.Contains(table.Name))
            {
                continue;
            }

            modelBuilder.SharedTypeEntity<Dictionary<string, object>>(
                $"DocsSchema_{schemaName}_{ToPascalCase(table.Name)}",
                entity =>
                {
                    entity.ToTable(table.Name, schemaName);

                    foreach (var column in table.Columns)
                    {
                        entity.IndexerProperty(InferClrType(column.SqlType), column.Name)
                            .HasColumnName(column.Name);
                    }

                    if (!string.IsNullOrWhiteSpace(table.PrimaryKeyColumn))
                    {
                        entity.HasKey(table.PrimaryKeyColumn);
                    }
                    else
                    {
                        entity.HasNoKey();
                    }
                });
        }
    }

    private static IReadOnlyList<SqlTable> ReadSchemaTables(string schemaName)
    {
        var assembly = typeof(DocsSchemaModelBuilderExtensions).Assembly;
        var resources = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);

        var tables = new List<SqlTable>();

        foreach (var resource in resources)
        {
            using var stream = assembly.GetManifestResourceStream(resource);
            if (stream is null)
            {
                continue;
            }

            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            tables.AddRange(ParseSqlTables(reader.ReadToEnd(), schemaName));
        }

        return tables;
    }

    private static IEnumerable<SqlTable> ParseSqlTables(string sql, string schemaName)
    {
        string? currentTable = null;
        var columns = new List<SqlColumn>();

        foreach (var rawLine in sql.Split(["\r\n", "\n"], StringSplitOptions.None))
        {
            var line = StripLineComment(rawLine).Trim();
            if (line.Length == 0)
            {
                continue;
            }

            if (currentTable is null)
            {
                var match = CreateTableRegex.Match(line);
                if (!match.Success)
                {
                    continue;
                }

                var tableName = match.Groups["table"].Value.Trim().Trim('"');
                if (!TrySplitQualifiedName(tableName, out var parsedSchema, out var parsedTable) ||
                    !string.Equals(parsedSchema, schemaName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                currentTable = parsedTable;
                columns = [];
                continue;
            }

            if (line.StartsWith(");", StringComparison.Ordinal))
            {
                yield return new SqlTable(
                    currentTable,
                    columns,
                    columns.FirstOrDefault(c => c.IsInlinePrimaryKey)?.Name ??
                    columns.FirstOrDefault(c => string.Equals(c.Name, "id", StringComparison.OrdinalIgnoreCase))?.Name);

                currentTable = null;
                columns = [];
                continue;
            }

            line = line.TrimEnd(',');
            var firstToken = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (firstToken is null || ConstraintKeywords.Contains(firstToken))
            {
                continue;
            }

            var columnMatch = ColumnRegex.Match(line);
            if (!columnMatch.Success)
            {
                continue;
            }

            columns.Add(new SqlColumn(
                columnMatch.Groups["name"].Value.Trim().Trim('"'),
                columnMatch.Groups["type"].Value.Trim(),
                line.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase)));
        }
    }

    private static bool TrySplitQualifiedName(string qualifiedName, out string schemaName, out string tableName)
    {
        var parts = qualifiedName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 2)
        {
            schemaName = parts[0].Trim('"');
            tableName = parts[1].Trim('"');
            return true;
        }

        schemaName = string.Empty;
        tableName = string.Empty;
        return false;
    }

    private static Type InferClrType(string sqlType)
    {
        var normalized = sqlType.Trim().ToUpperInvariant();

        if (normalized.StartsWith("UUID", StringComparison.Ordinal)) return typeof(Guid);
        if (normalized.StartsWith("SMALLINT", StringComparison.Ordinal)) return typeof(short);
        if (normalized.StartsWith("INTEGER", StringComparison.Ordinal)) return typeof(int);
        if (normalized.StartsWith("BIGINT", StringComparison.Ordinal)) return typeof(long);
        if (normalized.StartsWith("NUMERIC", StringComparison.Ordinal) || normalized.StartsWith("DECIMAL", StringComparison.Ordinal)) return typeof(decimal);
        if (normalized.StartsWith("REAL", StringComparison.Ordinal)) return typeof(float);
        if (normalized.StartsWith("DOUBLE", StringComparison.Ordinal)) return typeof(double);
        if (normalized.StartsWith("BOOLEAN", StringComparison.Ordinal)) return typeof(bool);
        if (normalized.StartsWith("DATE", StringComparison.Ordinal)) return typeof(DateOnly);
        if (normalized.StartsWith("TIMESTAMP", StringComparison.Ordinal) || normalized.StartsWith("TIMESTAMPTZ", StringComparison.Ordinal)) return typeof(DateTimeOffset);
        if (normalized.StartsWith("BYTEA", StringComparison.Ordinal)) return typeof(byte[]);

        return typeof(string);
    }

    private static string ToPascalCase(string value)
    {
        var builder = new StringBuilder(value.Length);
        var capitalizeNext = true;

        foreach (var character in value)
        {
            if (character == '_')
            {
                capitalizeNext = true;
                continue;
            }

            builder.Append(capitalizeNext
                ? char.ToUpperInvariant(character)
                : character);
            capitalizeNext = false;
        }

        return builder.ToString();
    }

    private static string StripLineComment(string line)
    {
        var commentStart = line.IndexOf("--", StringComparison.Ordinal);
        return commentStart >= 0 ? line[..commentStart] : line;
    }

    private sealed record SqlTable(string Name, IReadOnlyList<SqlColumn> Columns, string? PrimaryKeyColumn);

    private sealed record SqlColumn(string Name, string SqlType, bool IsInlinePrimaryKey);
}
