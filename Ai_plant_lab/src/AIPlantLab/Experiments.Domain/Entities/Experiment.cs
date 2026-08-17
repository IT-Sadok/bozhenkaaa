using Experiments.Domain.Common;
using Experiments.Domain.Enums;
using Experiments.Domain.Events;
using Experiments.Domain.ValueObjects;

namespace Experiments.Domain.Entities;

public sealed class Experiment : Entity
{
    public Guid Id { get; private init; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public ExperimentStatus Status { get; private set; }

    public ExperimentConfiguration? Configuration { get; private set; }

    public DateTime CreatedAt { get; private init; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? FinishedAt { get; private set; }

    private readonly List<PlantGroup> _plantGroups = [];

    public IReadOnlyCollection<PlantGroup> PlantGroups => _plantGroups.AsReadOnly();

    private Experiment()
    {
    }

    public static Result<Experiment> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Experiment>.ErrorResult("Experiment name is required.");
        }

        var experiment = new Experiment
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            Status = ExperimentStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        experiment.AddDomainEvent(new ExperimentCreatedDomainEvent(
            experiment.Id,
            experiment.Name,
            experiment.Description));

        return Result<Experiment>.Success(experiment);
    }

    public Result Configure(ExperimentConfiguration configuration)
    {
        if (Status != ExperimentStatus.Draft)
        {
            return Result.ErrorResult("Experiment can only be configured while in Draft status.");
        }

        if (configuration.LightHoursPerDay <= 0)
        {
            return Result.ErrorResult("Light hours per day must be greater than zero.");
        }

        if (configuration.WateringIntervalDays <= 0)
        {
            return Result.ErrorResult("Watering interval must be greater than zero.");
        }

        Configuration = configuration;
        Status = ExperimentStatus.Configured;

        AddDomainEvent(new ExperimentConfiguredDomainEvent(Id, configuration));

        return Result.Success();
    }

    public Result<PlantGroup> AddPlantGroup(string name, string species, int plantCount)
    {
        if (Status is ExperimentStatus.Running or ExperimentStatus.Finished)
        {
            return Result<PlantGroup>.ErrorResult("Cannot add plant groups to a running or finished experiment.");
        }

        if (Status is not (ExperimentStatus.Draft or ExperimentStatus.Configured))
        {
            return Result<PlantGroup>.ErrorResult("Plant groups can only be added while the experiment is Draft or Configured.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<PlantGroup>.ErrorResult("Plant group name is required.");
        }

        if (string.IsNullOrWhiteSpace(species))
        {
            return Result<PlantGroup>.ErrorResult("Species is required.");
        }

        if (plantCount <= 0)
        {
            return Result<PlantGroup>.ErrorResult("Plant count must be greater than zero.");
        }

        var plantGroup = PlantGroup.Create(Id, name, species, plantCount);
        _plantGroups.Add(plantGroup);

        AddDomainEvent(new PlantGroupAddedDomainEvent(
            Id,
            plantGroup.Id,
            plantGroup.Name,
            plantGroup.Species,
            plantGroup.PlantCount));

        return Result<PlantGroup>.Success(plantGroup);
    }

    public Result Start()
    {
        if (Status != ExperimentStatus.Configured)
        {
            return Result.ErrorResult("Experiment must be configured before it can be started.");
        }

        if (_plantGroups.Count == 0)
        {
            return Result.ErrorResult("At least one plant group is required to start the experiment.");
        }

        Status = ExperimentStatus.Running;
        StartedAt = DateTime.UtcNow;

        AddDomainEvent(new ExperimentStartedDomainEvent(Id, StartedAt.Value));

        return Result.Success();
    }

    public Result Finish()
    {
        if (Status != ExperimentStatus.Running)
        {
            return Result.ErrorResult("Only running experiments can be finished.");
        }

        Status = ExperimentStatus.Finished;
        FinishedAt = DateTime.UtcNow;

        AddDomainEvent(new ExperimentFinishedDomainEvent(Id, FinishedAt.Value));

        return Result.Success();
    }
}
