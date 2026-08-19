using FluentValidation;

namespace Experiments.Application.Commands.ConfigureExperiment;

internal sealed class ConfigureExperimentCommandValidator : AbstractValidator<ConfigureExperimentCommand>
{
    public ConfigureExperimentCommandValidator()
    {
        RuleFor(x => x.Configuration.LightHoursPerDay)
            .GreaterThan(0)
            .WithMessage("Light hours per day must be greater than zero.");

        RuleFor(x => x.Configuration.WateringIntervalDays)
            .GreaterThan(0)
            .WithMessage("Watering interval must be greater than zero.");
    }
}
