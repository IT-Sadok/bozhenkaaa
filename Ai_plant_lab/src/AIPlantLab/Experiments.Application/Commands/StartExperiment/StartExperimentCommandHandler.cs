using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Commands.StartExperiment;

internal sealed class StartExperimentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<StartExperimentCommand, Result>
{
    public async Task<Result> Handle(StartExperimentCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result.ErrorResult("Experiment not found.");
        }

        var startResult = experiment.Start();
        if (startResult.IsFailure)
        {
            return startResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
