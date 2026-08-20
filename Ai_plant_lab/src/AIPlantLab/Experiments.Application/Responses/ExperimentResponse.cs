namespace Experiments.Application.Responses;

public sealed class ExperimentResponse
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required string Status { get; init; }

    public ExperimentConfigurationResponse? Configuration { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? StartedAt { get; init; }

    public DateTime? FinishedAt { get; init; }

    public IReadOnlyList<PlantGroupResponse> PlantGroups { get; init; } = [];
}
