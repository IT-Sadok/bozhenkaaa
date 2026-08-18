using Experiments.Domain.Common;

namespace Experiments.Domain.Entities;

public sealed class PlantGroup : Entity
{
    public Guid Id { get; set; }

    public Guid ExperimentId { get; set; }

    public string Name { get; set; } = null!;

    public string Species { get; set; } = null!;

    public int PlantCount { get; set; }
}
