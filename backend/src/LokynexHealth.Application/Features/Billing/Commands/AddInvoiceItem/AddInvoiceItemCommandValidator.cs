using FluentValidation;

namespace LokynexHealth.Application.Features.Billing.Commands.AddInvoiceItem;

public class AddInvoiceItemCommandValidator : AbstractValidator<AddInvoiceItemCommand>
{
    public AddInvoiceItemCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500);

        RuleFor(x => x.SourceModule)
            .NotEmpty().WithMessage("SourceModule is required.")
            .MaximumLength(30);

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");

        RuleFor(x => x.GstRatePct)
            .InclusiveBetween(0, 100).WithMessage("GST rate must be between 0 and 100.");
    }
}
