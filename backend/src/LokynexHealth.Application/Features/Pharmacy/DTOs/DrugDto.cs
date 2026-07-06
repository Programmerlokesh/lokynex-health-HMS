namespace LokynexHealth.Application.Features.Pharmacy.DTOs;

public class DrugDto
{
    public Guid Id { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public bool IsJanAushadhiGeneric { get; set; }
    public string? HsnCode { get; set; }
    public decimal GstRatePct { get; set; }
    public string ScheduleFlag { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}