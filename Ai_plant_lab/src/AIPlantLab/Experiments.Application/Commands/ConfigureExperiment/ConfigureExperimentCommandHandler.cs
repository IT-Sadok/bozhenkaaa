using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using Experiments.Domain.Enums;
using Experiments.Domain.Events;
using Experiments.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Experiments.Application.Commands.ConfigureExperiment;

internal sealed class ConfigureExperimentCommandHandler(
    IValidator<ConfigureExperimentCommand> validator,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ConfigureExperimentCommand, Result>
{
    public async Task<Result> Handle(ConfigureExperimentCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result.Failure(ValidationMessages.NotFound, nameof(Experiment));
        }

        if (experiment.Status != ExperimentStatus.Draft)
        {
            return Result.Failure("Experiment can only be configured while in Draft status.");
        }

        var configuration = new ExperimentConfiguration(
            request.LightHoursPerDay,
            request.WateringIntervalDays,
            request.TargetTemperatureCelsius,
            request.Notes);

        experiment.Configuration = configuration;
        experiment.Status = ExperimentStatus.Configured;
        experiment.RaiseDomainEvent(new ExperimentConfiguredDomainEvent(experiment.Id, configuration));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
