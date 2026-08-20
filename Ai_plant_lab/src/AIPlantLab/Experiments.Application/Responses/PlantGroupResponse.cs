namespace Experiments.Application.Responses;

public sealed class PlantGroupResponse
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Species { get; init; }

    public int PlantCount { get; init; }
}
