namespace Experiments.Application.DTOs;

public sealed class PlantGroupDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Species { get; init; }

    public int PlantCount { get; init; }
}
