using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Commands.FinishExperiment;

internal sealed class FinishExperimentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<FinishExperimentCommand, Result>
{
    public async Task<Result> Handle(FinishExperimentCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result.ErrorResult("Experiment not found.");
        }

        var finishResult = experiment.Finish();
        if (finishResult.IsFailure)
        {
            return finishResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
