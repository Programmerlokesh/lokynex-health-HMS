using FluentValidation;

namespace LokynexHealth.Application.Features.Pharmacy.Commands.AddStockBatch;

public class AddStockBatchCommandValidator : AbstractValidator<AddStockBatchCommand>
{
    public AddStockBatchCommandValidator()
    {
        RuleFor(x => x.DrugId).NotEmpty();

        RuleFor(x => x.BatchNumber)
            .NotEmpty().WithMessage("Batch number is required.")
            .MaximumLength(50);

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Expiry date must be in the future.");

        RuleFor(x => x.QuantityReceived)
            .GreaterThan(0).WithMessage("Quantity received must be greater than zero.");

        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0).When(x => x.PurchasePrice.HasValue);
        RuleFor(x => x.Mrp).GreaterThanOrEqualTo(0).When(x => x.Mrp.HasValue);
    }
}