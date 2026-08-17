namespace Experiments.Domain.Events;

public sealed record PlantGroupAddedDomainEvent(
    Guid ExperimentId,
    Guid PlantGroupId,
    string Name,
    string Species,
    int PlantCount) : IDomainEvent;
