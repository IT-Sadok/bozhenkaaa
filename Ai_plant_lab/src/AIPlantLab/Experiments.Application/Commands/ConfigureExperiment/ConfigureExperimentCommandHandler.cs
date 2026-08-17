using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.ValueObjects;
using MediatR;

namespace Experiments.Application.Commands.ConfigureExperiment;

internal sealed class ConfigureExperimentCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ConfigureExperimentCommand, Result>
{
    public async Task<Result> Handle(ConfigureExperimentCommand request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.ExperimentId, cancellationToken);
        if (experiment is null)
        {
            return Result.ErrorResult("Experiment not found.");
        }

        var configuration = new ExperimentConfiguration(
            request.Configuration.LightHoursPerDay,
            request.Configuration.WateringIntervalDays,
            request.Configuration.TargetTemperatureCelsius,
            request.Configuration.Notes);

        var configureResult = experiment.Configure(configuration);
        if (configureResult.IsFailure)
        {
            return configureResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
