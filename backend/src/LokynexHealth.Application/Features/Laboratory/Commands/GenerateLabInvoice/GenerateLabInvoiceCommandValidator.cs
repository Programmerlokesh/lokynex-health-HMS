using FluentValidation;

namespace LokynexHealth.Application.Features.Laboratory.Commands.GenerateLabInvoice;

public class GenerateLabInvoiceCommandValidator : AbstractValidator<GenerateLabInvoiceCommand>
{
    public GenerateLabInvoiceCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();

        RuleFor(x => x.GstRatePct)
            .InclusiveBetween(0, 100).WithMessage("GST rate must be between 0 and 100.");
    }
}