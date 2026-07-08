using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LokynexHealth.API.Swagger;

/// <summary>
/// Adds an "X-Tenant-Subdomain" header input field to every endpoint in Swagger UI,
/// except platform-level (SuperAdmin) endpoints which bypass tenant resolution.
/// This mirrors the skip-list in TenantResolutionMiddleware.
/// </summary>
public class TenantHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var relativePath = context.ApiDescription.RelativePath ?? string.Empty;

        if (relativePath.StartsWith("api/PlatformTenants", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        operation.Parameters ??= new List<Microsoft.OpenApi.IOpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Tenant-Subdomain",
            In = ParameterLocation.Header,
            Required = true,
            Description = "The tenant's subdomain (e.g. 'apollo-kolkata'). Required for all tenant-scoped endpoints.",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String }
        });
    }
}
