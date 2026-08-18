using Experiments.Application.Interfaces;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Entities;
using Experiments.Domain.Enums;
using Experiments.Domain.Events;
using FluentValidation;
using MediatR;

namespace Experiments.Application.Commands.FinishExperiment;

internal sealed class FinishExperimentCommandHandler(
    IValidator<FinishExperimentCommand> validator,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTime)
    : IRequestHandler<FinishExperimentCommand, Result>
{
    public async Task<Result> Handle(FinishExperimentCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);

        var context = new ValidationContext<FinishExperimentCommand>(request);
        context.RootContextData[nameof(Experiment)] = experiment;

        var validation = await validator.ValidateAsync(context, cancellationToken);
        if (!validation.IsValid)
        {
            var error = validation.Errors[0];
            return Result.Failure(error.ErrorMessage, error.CustomState as string);
        }

        var finishedAt = dateTime.UtcNow;
        experiment!.Status = ExperimentStatus.Finished;
        experiment.FinishedAt = finishedAt;
        experiment.RaiseDomainEvent(new ExperimentFinishedDomainEvent(experiment.Id, finishedAt));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
