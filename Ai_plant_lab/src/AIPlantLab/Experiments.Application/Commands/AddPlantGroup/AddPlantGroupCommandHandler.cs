using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Entities;
using Experiments.Domain.Events;
using FluentValidation;
using MediatR;

namespace Experiments.Application.Commands.AddPlantGroup;

internal sealed class AddPlantGroupCommandHandler(
    IValidator<AddPlantGroupCommand> validator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddPlantGroupCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddPlantGroupCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);

        var context = new ValidationContext<AddPlantGroupCommand>(request);
        context.RootContextData[nameof(Experiment)] = experiment;

        var validation = await validator.ValidateAsync(context, cancellationToken);
        if (!validation.IsValid)
        {
            var error = validation.Errors[0];
            return Result<Guid>.Failure(error.ErrorMessage, error.CustomState as string);
        }

        var plantGroup = new PlantGroup
        {
            Id = Guid.NewGuid(),
            ExperimentId = experiment!.Id,
            Name = request.Name.Trim(),
            Species = request.Species.Trim(),
            PlantCount = request.PlantCount
        };

        experiment.PlantGroups.Add(plantGroup);
        experiment.RaiseDomainEvent(new PlantGroupAddedDomainEvent(
            experiment.Id,
            plantGroup.Id,
            plantGroup.Name,
            plantGroup.Species,
            plantGroup.PlantCount));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(plantGroup.Id);
    }
}
