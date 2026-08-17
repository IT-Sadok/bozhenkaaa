using Experiments.Domain.Common;

namespace Experiments.Domain.Entities;

public sealed class PlantGroup : Entity
{
    public Guid Id { get; private init; }

    public Guid ExperimentId { get; private init; }

    public string Name { get; private init; } = null!;

    public string Species { get; private init; } = null!;

    public int PlantCount { get; private init; }

    private PlantGroup()
    {
    }

    internal static PlantGroup Create(Guid experimentId, string name, string species, int plantCount)
    {
        return new PlantGroup
        {
            Id = Guid.NewGuid(),
            ExperimentId = experimentId,
            Name = name.Trim(),
            Species = species.Trim(),
            PlantCount = plantCount
        };
    }
}
