using FluentValidation;

namespace Experiments.Application.Commands.ConfigureExperiment;

internal sealed class ConfigureExperimentCommandValidator : AbstractValidator<ConfigureExperimentCommand>
{
    public ConfigureExperimentCommandValidator()
    {
        RuleFor(x => x.LightHoursPerDay)
            .GreaterThan(0)
            .WithMessage("Light hours per day must be greater than zero.");

        RuleFor(x => x.WateringIntervalDays)
            .GreaterThan(0)
            .WithMessage("Watering interval must be greater than zero.");
    }
}
