namespace Experiments.Application.DTOs;

public sealed class ExperimentSummaryDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Status { get; init; }

    public int PlantGroupCount { get; init; }

    public DateTime CreatedAt { get; init; }
}
