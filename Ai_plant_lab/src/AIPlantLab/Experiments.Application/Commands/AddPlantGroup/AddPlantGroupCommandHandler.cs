using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using Experiments.Domain.Services;
using MediatR;

namespace Experiments.Application.Commands.AddPlantGroup;

internal sealed class AddPlantGroupCommandHandler(
    IUnitOfWork unitOfWork,
    IExperimentService experimentService)
    : IRequestHandler<AddPlantGroupCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddPlantGroupCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result<Guid>.Failure(ValidationMessages.NotFound, source: nameof(Experiment));
        }

        var addResult = experimentService.AddPlantGroup(
            experiment,
            request.Name,
            request.Species,
            request.PlantCount);

        if (addResult.IsFailure)
        {
            return Result<Guid>.Failure(addResult.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(addResult.Value!.Id);
    }
}
