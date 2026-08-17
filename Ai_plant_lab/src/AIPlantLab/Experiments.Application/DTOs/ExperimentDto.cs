namespace Experiments.Application.DTOs;

public sealed class ExperimentConfigurationDto
{
    public int LightHoursPerDay { get; init; }

    public int WateringIntervalDays { get; init; }

    public decimal TargetTemperatureCelsius { get; init; }

    public string? Notes { get; init; }
}

public sealed class PlantGroupDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Species { get; init; }

    public int PlantCount { get; init; }
}

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

public sealed class ExperimentSummaryDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Status { get; init; }

    public int PlantGroupCount { get; init; }

    public DateTime CreatedAt { get; init; }
}

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}
