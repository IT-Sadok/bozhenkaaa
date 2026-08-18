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

    public int PlantGroupCount => _plantGroups.Count;

    private Experiment()
    {
    }

    internal static Experiment CreateNew(string name, string? description)
    {
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

        return experiment;
    }

    internal void SetConfigured(ExperimentConfiguration configuration)
    {
        Configuration = configuration;
        Status = ExperimentStatus.Configured;
        AddDomainEvent(new ExperimentConfiguredDomainEvent(Id, configuration));
    }

    internal void AddPlantGroup(PlantGroup plantGroup)
    {
        _plantGroups.Add(plantGroup);
        AddDomainEvent(new PlantGroupAddedDomainEvent(
            Id,
            plantGroup.Id,
            plantGroup.Name,
            plantGroup.Species,
            plantGroup.PlantCount));
    }

    internal void SetRunning(DateTime startedAt)
    {
        Status = ExperimentStatus.Running;
        StartedAt = startedAt;
        AddDomainEvent(new ExperimentStartedDomainEvent(Id, startedAt));
    }

    internal void SetFinished(DateTime finishedAt)
    {
        Status = ExperimentStatus.Finished;
        FinishedAt = finishedAt;
        AddDomainEvent(new ExperimentFinishedDomainEvent(Id, finishedAt));
    }
}
