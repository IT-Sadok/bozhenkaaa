using Experiments.Domain.Common;
using Experiments.Domain.Entities;
using Experiments.Domain.Enums;
using Experiments.Domain.ValueObjects;

namespace Experiments.Domain.Services;

public sealed class ExperimentService : IExperimentService
{
    public Result<Experiment> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Experiment>.Failure("Experiment name is required.");
        }

        var experiment = Experiment.CreateNew(name, description);

        return Result<Experiment>.Success(experiment);
    }

    public Result Configure(Experiment experiment, ExperimentConfiguration configuration)
    {
        if (experiment.Status != ExperimentStatus.Draft)
        {
            return Result.Failure("Experiment can only be configured while in Draft status.");
        }

        if (configuration.LightHoursPerDay <= 0)
        {
            return Result.Failure("Light hours per day must be greater than zero.");
        }

        if (configuration.WateringIntervalDays <= 0)
        {
            return Result.Failure("Watering interval must be greater than zero.");
        }

        experiment.SetConfigured(configuration);

        return Result.Success();
    }

    public Result<PlantGroup> AddPlantGroup(Experiment experiment, string name, string species, int plantCount)
    {
        if (experiment.Status is ExperimentStatus.Running or ExperimentStatus.Finished)
        {
            return Result<PlantGroup>.Failure("Cannot add plant groups to a running or finished experiment.");
        }

        if (experiment.Status is not (ExperimentStatus.Draft or ExperimentStatus.Configured))
        {
            return Result<PlantGroup>.Failure("Plant groups can only be added while the experiment is Draft or Configured.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<PlantGroup>.Failure("Plant group name is required.");
        }

        if (string.IsNullOrWhiteSpace(species))
        {
            return Result<PlantGroup>.Failure("Species is required.");
        }

        if (plantCount <= 0)
        {
            return Result<PlantGroup>.Failure("Plant count must be greater than zero.");
        }

        var plantGroup = PlantGroup.Create(experiment.Id, name, species, plantCount);
        experiment.AddPlantGroup(plantGroup);

        return Result<PlantGroup>.Success(plantGroup);
    }

    public Result Start(Experiment experiment)
    {
        if (experiment.Status != ExperimentStatus.Configured)
        {
            return Result.Failure("Experiment must be configured before it can be started.");
        }

        if (experiment.PlantGroupCount == 0)
        {
            return Result.Failure("At least one plant group is required to start the experiment.");
        }

        experiment.SetRunning(DateTime.UtcNow);

        return Result.Success();
    }

    public Result Finish(Experiment experiment)
    {
        if (experiment.Status != ExperimentStatus.Running)
        {
            return Result.Failure("Only running experiments can be finished.");
        }

        experiment.SetFinished(DateTime.UtcNow);

        return Result.Success();
    }
}
