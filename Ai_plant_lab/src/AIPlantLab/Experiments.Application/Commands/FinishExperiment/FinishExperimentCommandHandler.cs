using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using Experiments.Domain.Services;
using MediatR;

namespace Experiments.Application.Commands.FinishExperiment;

internal sealed class FinishExperimentCommandHandler(
    IUnitOfWork unitOfWork,
    IExperimentService experimentService)
    : IRequestHandler<FinishExperimentCommand, Result>
{
    public async Task<Result> Handle(FinishExperimentCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result.Failure(ValidationMessages.NotFound, source: nameof(Experiment));
        }

        var finishResult = experimentService.Finish(experiment);
        if (finishResult.IsFailure)
        {
            return finishResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
