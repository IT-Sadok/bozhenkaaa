using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
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
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);

        var context = new ValidationContext<ConfigureExperimentCommand>(request);
        context.RootContextData[nameof(Experiment)] = experiment;

        var validation = await validator.ValidateAsync(context, cancellationToken);
        if (!validation.IsValid)
        {
            var error = validation.Errors[0];
            return Result.Failure(error.ErrorMessage, error.CustomState as string);
        }

        var configuration = new ExperimentConfiguration(
            request.Configuration.LightHoursPerDay,
            request.Configuration.WateringIntervalDays,
            request.Configuration.TargetTemperatureCelsius,
            request.Configuration.Notes);

        experiment!.Configuration = configuration;
        experiment.Status = ExperimentStatus.Configured;
        experiment.RaiseDomainEvent(new ExperimentConfiguredDomainEvent(experiment.Id, configuration));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
