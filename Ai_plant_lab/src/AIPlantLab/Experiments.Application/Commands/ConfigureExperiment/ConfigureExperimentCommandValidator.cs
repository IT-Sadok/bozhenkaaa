using Experiments.Application.Common;
using Experiments.Domain.Enums;
using FluentValidation;

namespace Experiments.Application.Commands.ConfigureExperiment;

internal sealed class ConfigureExperimentCommandValidator : AbstractValidator<ConfigureExperimentCommand>
{
    public ConfigureExperimentCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .MustExistInContext();

        RuleFor(x => x.Configuration.LightHoursPerDay)
            .GreaterThan(0)
            .WithMessage("Light hours per day must be greater than zero.");

        RuleFor(x => x.Configuration.WateringIntervalDays)
            .GreaterThan(0)
            .WithMessage("Watering interval must be greater than zero.");

        RuleFor(x => x)
            .Must((_, _, ctx) => ctx.GetExperiment() is { Status: ExperimentStatus.Draft })
            .WithMessage("Experiment can only be configured while in Draft status.");
    }
}
