namespace Experiments.Domain.Events;

public sealed record ExperimentFinishedDomainEvent(
    Guid ExperimentId,
    string Name,
    DateTime FinishedAt) : IDomainEvent;
