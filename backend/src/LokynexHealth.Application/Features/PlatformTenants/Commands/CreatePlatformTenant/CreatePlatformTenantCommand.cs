using MediatR;
using LokynexHealth.Domain.Platform.Enums;

namespace LokynexHealth.Application.Features.PlatformTenants.Commands.CreatePlatformTenant;

public class CreatePlatformTenantCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public FacilityType FacilityType { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Gstin { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string Subdomain { get; set; } = string.Empty;
}
