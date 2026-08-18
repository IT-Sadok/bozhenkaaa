using FluentValidation;

namespace Experiments.Application.Commands.CreateExperiment;

internal sealed class CreateExperimentCommandValidator : AbstractValidator<CreateExperimentCommand>
{
    public CreateExperimentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Experiment name is required.");
    }
}
