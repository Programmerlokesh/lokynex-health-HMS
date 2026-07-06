namespace LokynexHealth.Application.Features.Billing.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public DateTimeOffset PaidAt { get; set; }
}
