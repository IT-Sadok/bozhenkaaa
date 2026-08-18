using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using Experiments.Domain.Services;
using Experiments.Domain.ValueObjects;
using MediatR;

namespace Experiments.Application.Commands.ConfigureExperiment;

internal sealed class ConfigureExperimentCommandHandler(
    IUnitOfWork unitOfWork,
    IExperimentService experimentService)
    : IRequestHandler<ConfigureExperimentCommand, Result>
{
    public async Task<Result> Handle(ConfigureExperimentCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result.Failure(ValidationMessages.NotFound, source: nameof(Experiment));
        }

        var configuration = new ExperimentConfiguration(
            request.Configuration.LightHoursPerDay,
            request.Configuration.WateringIntervalDays,
            request.Configuration.TargetTemperatureCelsius,
            request.Configuration.Notes);

        var configureResult = experimentService.Configure(experiment, configuration);
        if (configureResult.IsFailure)
        {
            return configureResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
