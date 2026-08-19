using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using Experiments.Domain.Enums;
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
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<Guid>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result<Guid>.Failure(ValidationMessages.NotFound, nameof(Experiment));
        }

        if (experiment.Status is not (ExperimentStatus.Draft or ExperimentStatus.Configured))
        {
            return Result<Guid>.Failure(
                "Plant groups can only be added while the experiment is Draft or Configured.");
        }

        var plantGroup = new PlantGroup
        {
            Id = Guid.NewGuid(),
            ExperimentId = experiment.Id,
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
