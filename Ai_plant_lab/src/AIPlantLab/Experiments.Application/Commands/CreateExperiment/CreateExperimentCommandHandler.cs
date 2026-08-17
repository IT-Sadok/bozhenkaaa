using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Entities;
using MediatR;

namespace Experiments.Application.Commands.CreateExperiment;

internal sealed class CreateExperimentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateExperimentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateExperimentCommand request, CancellationToken cancellationToken)
    {
        var createResult = Experiment.Create(request.Name, request.Description);
        if (createResult.IsFailure)
        {
            return Result<Guid>.ErrorResult(createResult.Error);
        }

        var experiment = createResult.Value!;
        unitOfWork.Experiments.Add(experiment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(experiment.Id);
    }
}
