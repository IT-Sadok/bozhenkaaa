namespace Experiments.Domain.Events;

public sealed record ExperimentCreatedDomainEvent(
    Guid ExperimentId,
    string Name,
    string? Description) : IDomainEvent;
