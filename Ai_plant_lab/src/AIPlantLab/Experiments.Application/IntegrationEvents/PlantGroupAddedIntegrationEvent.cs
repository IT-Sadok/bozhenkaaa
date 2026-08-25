namespace Experiments.Application.IntegrationEvents;

public sealed record PlantGroupAddedIntegrationEvent(
    Guid ExperimentId,
    Guid PlantGroupId,
    string Name,
    string Species,
    int PlantCount,
    DateTime OccurredAt);
