using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using Experiments.Domain.Services;
using MediatR;

namespace Experiments.Application.Commands.StartExperiment;

internal sealed class StartExperimentCommandHandler(
    IUnitOfWork unitOfWork,
    IExperimentService experimentService)
    : IRequestHandler<StartExperimentCommand, Result>
{
    public async Task<Result> Handle(StartExperimentCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result.Failure(ValidationMessages.NotFound, source: nameof(Experiment));
        }

        var startResult = experimentService.Start(experiment);
        if (startResult.IsFailure)
        {
            return startResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
