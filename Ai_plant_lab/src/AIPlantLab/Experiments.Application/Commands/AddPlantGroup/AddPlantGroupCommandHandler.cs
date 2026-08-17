using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Commands.AddPlantGroup;

internal sealed class AddPlantGroupCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AddPlantGroupCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddPlantGroupCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result<Guid>.ErrorResult("Experiment not found.");
        }

        var addResult = experiment.AddPlantGroup(request.Name, request.Species, request.PlantCount);
        if (addResult.IsFailure)
        {
            return Result<Guid>.ErrorResult(addResult.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(addResult.Value!.Id);
    }
}
