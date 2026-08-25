namespace Experiments.Domain.Events;

public sealed record ExperimentFinishedDomainEvent(
    Guid ExperimentId,
    DateTime FinishedAt) : IDomainEvent;
