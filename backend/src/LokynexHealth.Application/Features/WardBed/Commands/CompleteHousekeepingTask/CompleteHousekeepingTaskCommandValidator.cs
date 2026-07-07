using FluentValidation;

namespace LokynexHealth.Application.Features.WardBed.Commands.CompleteHousekeepingTask;

public class CompleteHousekeepingTaskCommandValidator : AbstractValidator<CompleteHousekeepingTaskCommand>
{
    public CompleteHousekeepingTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}