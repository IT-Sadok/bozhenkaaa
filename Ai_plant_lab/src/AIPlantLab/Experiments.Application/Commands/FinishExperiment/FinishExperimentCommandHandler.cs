using Experiments.Application.Interfaces;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using Experiments.Domain.Enums;
using Experiments.Domain.Events;
using MediatR;

namespace Experiments.Application.Commands.FinishExperiment;

internal sealed class FinishExperimentCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTime)
    : IRequestHandler<FinishExperimentCommand, Result>
{
    public async Task<Result> Handle(FinishExperimentCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result.Failure(ValidationMessages.NotFound, nameof(Experiment));
        }

        if (experiment.Status != ExperimentStatus.Running)
        {
            return Result.Failure("Only running experiments can be finished.");
        }

        var finishedAt = dateTime.UtcNow;
        experiment.Status = ExperimentStatus.Finished;
        experiment.FinishedAt = finishedAt;
        experiment.RaiseDomainEvent(new ExperimentFinishedDomainEvent(experiment.Id, experiment.Name, finishedAt));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
