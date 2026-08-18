namespace Experiments.Application.DTOs;

public sealed class ExperimentDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required string Status { get; init; }

    public ExperimentConfigurationDto? Configuration { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? StartedAt { get; init; }

    public DateTime? FinishedAt { get; init; }

    public IReadOnlyList<PlantGroupDto> PlantGroups { get; init; } = [];
}
