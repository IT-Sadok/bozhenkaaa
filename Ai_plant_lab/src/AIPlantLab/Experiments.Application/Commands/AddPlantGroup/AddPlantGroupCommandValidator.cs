using Experiments.Application.Common;
using Experiments.Domain.Enums;
using FluentValidation;

namespace Experiments.Application.Commands.AddPlantGroup;

internal sealed class AddPlantGroupCommandValidator : AbstractValidator<AddPlantGroupCommand>
{
    public AddPlantGroupCommandValidator()
    {
        RuleFor(x => x.ExperimentId)
            .MustExistInContext();

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Plant group name is required.");

        RuleFor(x => x.Species)
            .NotEmpty()
            .WithMessage("Species is required.");

        RuleFor(x => x.PlantCount)
            .GreaterThan(0)
            .WithMessage("Plant count must be greater than zero.");

        RuleFor(x => x)
            .Must((_, _, ctx) => ctx.GetExperiment() is
            {
                Status: ExperimentStatus.Draft or ExperimentStatus.Configured
            })
            .WithMessage("Plant groups can only be added while the experiment is Draft or Configured.");
    }
}
