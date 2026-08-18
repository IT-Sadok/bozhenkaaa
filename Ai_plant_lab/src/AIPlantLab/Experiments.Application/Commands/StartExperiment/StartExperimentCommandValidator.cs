using Experiments.Application.Common;
using Experiments.Domain.Enums;
using FluentValidation;

namespace Experiments.Application.Commands.StartExperiment;

internal sealed class StartExperimentCommandValidator : AbstractValidator<StartExperimentCommand>
{
    public StartExperimentCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .MustExistInContext();

        RuleFor(x => x)
            .Must((_, _, ctx) => ctx.GetExperiment() is { Status: ExperimentStatus.Configured })
            .WithMessage("Experiment must be configured before it can be started.");

        RuleFor(x => x)
            .Must((_, _, ctx) => ctx.GetExperiment() is { PlantGroupCount: > 0 })
            .WithMessage("At least one plant group is required to start the experiment.");
    }
}
