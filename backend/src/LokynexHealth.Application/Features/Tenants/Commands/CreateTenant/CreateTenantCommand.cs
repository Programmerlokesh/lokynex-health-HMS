using MediatR;

namespace LokynexHealth.Application.Features.Tenants.Commands.CreateTenant;

public class CreateTenantCommand : IRequest<Guid>
{
    public string HospitalName { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
