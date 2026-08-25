using Experiments.Domain.Common;
using Experiments.Domain.Enums;
using Experiments.Domain.ValueObjects;

namespace Experiments.Domain.Entities;

public sealed class Experiment : Entity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ExperimentStatus Status { get; set; }

    public ExperimentConfiguration? Configuration { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    private readonly List<PlantGroup> _plantGroups = [];

    public ICollection<PlantGroup> PlantGroups => _plantGroups;

    public int PlantGroupCount => _plantGroups.Count;
}
