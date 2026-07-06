using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using LokynexHealth.Application.Common.Interfaces;
using LokynexHealth.Application.Common.Models;

namespace LokynexHealth.Infrastructure.Services;

public class ModuleSchemaService : IModuleSchemaService
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

    private static readonly IReadOnlyList<ModuleDefinition> ModuleDefinitions =
    [
        new("shared", "Shared Extensions & AI Audit", "hms", "00_extensions_common.sql", 0, []),
        new("platform-superadmin", "Platform SuperAdmin", "superadmin", "01_platform_superadmin.sql", 1, []),
        new("patient-registration", "Patient Registration", "hms", "02_patient_registration.sql", 2, ["shared"]),
        new("doctor-opd", "Doctor / OPD", "hms", "03_doctor_opd.sql", 3, ["patient-registration"]),
        new("laboratory", "Laboratory", "hms", "04_laboratory.sql", 4, ["patient-registration"]),
        new("pharmacy-pos", "Pharmacy POS", "hms", "05_pharmacy_pos.sql", 5, ["patient-registration"]),
        new("ward-bed-management", "Ward & Bed Management", "hms", "06_ward_bed_management.sql", 6, ["patient-registration"]),
        new("icu-monitoring", "ICU Monitoring", "hms", "07_icu_monitoring.sql", 7, ["ward-bed-management"]),
        new("emergency-er", "Emergency / ER", "hms", "08_emergency_er.sql", 8, ["patient-registration"]),
        new("billing-finance", "Billing & Finance", "hms", "09_billing_finance.sql", 9, ["ward-bed-management"]),
        new("reports-nabh", "Reports & NABH", "hms", "10_reports_nabh.sql", 10, ["shared"]),
        new("ot-management", "OT Management", "hms", "11_ot_management.sql", 11, ["ward-bed-management"]),
        new("blood-bank", "Blood Bank", "hms", "12_blood_bank.sql", 12, ["patient-registration"]),
        new("radiology-pacs", "Radiology / PACS", "hms", "13_radiology_pacs.sql", 13, ["patient-registration", "doctor-opd"]),
        new("hr-payroll", "HR & Payroll", "hms", "14_hr_payroll.sql", 14, ["shared"]),
        new("patient-portal-telemedicine", "Patient Portal & Telemedicine", "hms", "15_patient_portal_telemedicine.sql", 15, ["patient-registration", "doctor-opd", "billing-finance"])
    ];

    private readonly Lazy<IReadOnlyList<HmsModuleDto>> _modules = new(BuildModules);

    public IReadOnlyList<HmsModuleDto> GetModules()
    {
        return _modules.Value;
    }

    public HmsModuleDto? GetModule(string moduleKey)
    {
        return GetModules().FirstOrDefault(module =>
            string.Equals(module.Key, moduleKey, StringComparison.OrdinalIgnoreCase));
    }

    public ModuleTableDto? GetTable(string moduleKey, string tableName)
    {
        return GetModule(moduleKey)?.Tables.FirstOrDefault(table =>
            string.Equals(table.Name, tableName, StringComparison.OrdinalIgnoreCase));
    }

    public bool TableBelongsToModule(string moduleKey, string tableName)
    {
        return GetTable(moduleKey, tableName) is not null;
    }

    private static IReadOnlyList<HmsModuleDto> BuildModules()
    {
        return ModuleDefinitions
            .Select(definition => new HmsModuleDto
            {
                Key = definition.Key,
                Name = definition.Name,
                SchemaName = definition.SchemaName,
                FileName = definition.FileName,
                SortOrder = definition.SortOrder,
                DependsOn = definition.DependsOn.ToList(),
                Tables = ReadTables(definition).ToList()
            })
            .OrderBy(module => module.SortOrder)
            .ToList();
    }

    private static IEnumerable<ModuleTableDto> ReadTables(ModuleDefinition definition)
    {
        var sql = ReadEmbeddedSql(definition.FileName);
        string? currentTable = null;
        List<ModuleColumnDto> columns = [];

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
                var parts = tableName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length != 2 || !string.Equals(parts[0].Trim('"'), definition.SchemaName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                currentTable = parts[1].Trim('"');
                columns = [];
                continue;
            }

            if (line.StartsWith(");", StringComparison.Ordinal))
            {
                yield return new ModuleTableDto
                {
                    Name = currentTable,
                    Columns = columns
                };

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

            var sqlType = columnMatch.Groups["type"].Value.Trim();
            columns.Add(new ModuleColumnDto
            {
                Name = columnMatch.Groups["name"].Value.Trim().Trim('"'),
                SqlType = sqlType,
                HasDefault = sqlType.Contains(" DEFAULT ", StringComparison.OrdinalIgnoreCase),
                IsNullable = !sqlType.Contains(" NOT NULL", StringComparison.OrdinalIgnoreCase) &&
                    !sqlType.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase),
                IsPrimaryKey = sqlType.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase)
            });
        }
    }

    private static string ReadEmbeddedSql(string fileName)
    {
        var assembly = typeof(ModuleSchemaService).Assembly;
        var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(name =>
            name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            throw new InvalidOperationException($"Embedded schema resource '{fileName}' was not found.");
        }

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded schema resource '{fileName}' could not be opened.");
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
    }

    private static string StripLineComment(string line)
    {
        var commentStart = line.IndexOf("--", StringComparison.Ordinal);
        return commentStart >= 0 ? line[..commentStart] : line;
    }

    private sealed record ModuleDefinition(
        string Key,
        string Name,
        string SchemaName,
        string FileName,
        int SortOrder,
        IReadOnlyList<string> DependsOn);
}
