using FluentValidation;

namespace Experiments.Application.Commands.AddPlantGroup;

internal sealed class AddPlantGroupCommandValidator : AbstractValidator<AddPlantGroupCommand>
{
    public AddPlantGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Plant group name is required.");

        RuleFor(x => x.Species)
            .NotEmpty()
            .WithMessage("Species is required.");

        RuleFor(x => x.PlantCount)
            .GreaterThan(0)
            .WithMessage("Plant count must be greater than zero.");
    }
}
