using Experiments.Application.Common;
using Experiments.Domain.Enums;
using FluentValidation;

namespace Experiments.Application.Commands.FinishExperiment;

internal sealed class FinishExperimentCommandValidator : AbstractValidator<FinishExperimentCommand>
{
    public FinishExperimentCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .MustExistInContext();

        RuleFor(x => x)
            .Must((_, _, ctx) => ctx.GetExperiment() is { Status: ExperimentStatus.Running })
            .WithMessage("Only running experiments can be finished.");
    }
}
