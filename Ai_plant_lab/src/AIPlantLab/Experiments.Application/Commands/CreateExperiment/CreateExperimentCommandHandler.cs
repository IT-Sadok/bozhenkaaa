using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Services;
using MediatR;

namespace Experiments.Application.Commands.CreateExperiment;

internal sealed class CreateExperimentCommandHandler(
    IUnitOfWork unitOfWork,
    IExperimentService experimentService)
    : IRequestHandler<CreateExperimentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateExperimentCommand request, CancellationToken cancellationToken)
    {
        var createResult = experimentService.Create(request.Name, request.Description);
        if (createResult.IsFailure)
        {
            return Result<Guid>.Failure(createResult.Error);
        }

        var experiment = createResult.Value!;
        unitOfWork.Experiments.Add(experiment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(experiment.Id);
    }
}
