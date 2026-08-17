namespace Experiments.Domain.Events;

public sealed record ExperimentStartedDomainEvent(
    Guid ExperimentId,
    DateTime StartedAt) : IDomainEvent;
