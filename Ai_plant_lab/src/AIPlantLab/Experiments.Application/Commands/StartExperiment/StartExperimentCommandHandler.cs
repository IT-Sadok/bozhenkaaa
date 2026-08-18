using Experiments.Application.Interfaces;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Entities;
using Experiments.Domain.Enums;
using Experiments.Domain.Events;
using FluentValidation;
using MediatR;

namespace Experiments.Application.Commands.StartExperiment;

internal sealed class StartExperimentCommandHandler(
    IValidator<StartExperimentCommand> validator,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTime)
    : IRequestHandler<StartExperimentCommand, Result>
{
    public async Task<Result> Handle(StartExperimentCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);

        var context = new ValidationContext<StartExperimentCommand>(request);
        context.RootContextData[nameof(Experiment)] = experiment;

        var validation = await validator.ValidateAsync(context, cancellationToken);
        if (!validation.IsValid)
        {
            var error = validation.Errors[0];
            return Result.Failure(error.ErrorMessage, error.CustomState as string);
        }

        var startedAt = dateTime.UtcNow;
        experiment!.Status = ExperimentStatus.Running;
        experiment.StartedAt = startedAt;
        experiment.RaiseDomainEvent(new ExperimentStartedDomainEvent(experiment.Id, startedAt));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
