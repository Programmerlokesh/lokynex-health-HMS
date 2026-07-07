using FluentValidation;

namespace LokynexHealth.Application.Features.WardBed.Commands.RecordNursingAssessment;

public class RecordNursingAssessmentCommandValidator : AbstractValidator<RecordNursingAssessmentCommand>
{
    public RecordNursingAssessmentCommandValidator()
    {
        RuleFor(x => x.AdmissionId).NotEmpty();
        RuleFor(x => x.GcsScore).InclusiveBetween((short)3, (short)15).When(x => x.GcsScore.HasValue);
        RuleFor(x => x.BradenScore).InclusiveBetween((short)6, (short)23).When(x => x.BradenScore.HasValue);
        RuleFor(x => x.MorseFallScore).InclusiveBetween((short)0, (short)125).When(x => x.MorseFallScore.HasValue);
        RuleFor(x => x.Nrs2002Score).InclusiveBetween((short)0, (short)7).When(x => x.Nrs2002Score.HasValue);
    }
}