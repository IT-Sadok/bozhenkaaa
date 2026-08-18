using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using FluentValidation;

namespace Experiments.Application.Common;

internal static class ExperimentValidationExtensions
{
    public static Experiment? GetExperiment(this IValidationContext context)
    {
        if (context.RootContextData.TryGetValue(nameof(Experiment), out var value))
        {
            return value as Experiment;
        }

        return null;
    }

    public static void MustExistInContext<T>(this IRuleBuilder<T, Guid> ruleBuilder)
    {
        ruleBuilder
            .Must((_, _, ctx) => ctx.GetExperiment() is not null)
            .WithMessage(ValidationMessages.NotFound)
            .WithState(_ => nameof(Experiment));
    }
}
