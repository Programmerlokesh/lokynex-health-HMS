using LokynexHealth.Application.Common.Models;

namespace LokynexHealth.Application.Common.Interfaces;

public interface IModuleSchemaService
{
    IReadOnlyList<HmsModuleDto> GetModules();
    HmsModuleDto? GetModule(string moduleKey);
    ModuleTableDto? GetTable(string moduleKey, string tableName);
    bool TableBelongsToModule(string moduleKey, string tableName);
}
