using Experiments.Application.Interfaces;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using Experiments.Domain.Enums;
using Experiments.Domain.Events;
using MediatR;

namespace Experiments.Application.Commands.StartExperiment;

internal sealed class StartExperimentCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTime)
    : IRequestHandler<StartExperimentCommand, Result>
{
    public async Task<Result> Handle(StartExperimentCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result.Failure(ValidationMessages.NotFound, nameof(Experiment));
        }

        if (experiment.Status != ExperimentStatus.Configured)
        {
            return Result.Failure("Experiment must be configured before it can be started.");
        }

        if (experiment.PlantGroupCount == 0)
        {
            return Result.Failure("At least one plant group is required to start the experiment.");
        }

        var startedAt = dateTime.UtcNow;
        experiment.Status = ExperimentStatus.Running;
        experiment.StartedAt = startedAt;
        experiment.RaiseDomainEvent(new ExperimentStartedDomainEvent(experiment.Id, startedAt));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
