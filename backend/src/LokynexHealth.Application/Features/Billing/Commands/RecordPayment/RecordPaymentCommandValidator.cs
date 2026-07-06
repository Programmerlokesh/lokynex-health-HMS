using FluentValidation;

namespace LokynexHealth.Application.Features.Billing.Commands.RecordPayment;

public class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than zero.");

        RuleFor(x => x.Method).IsInEnum();

        RuleFor(x => x.ReferenceNumber)
            .MaximumLength(100)
            .When(x => x.ReferenceNumber is not null);
    }
}
