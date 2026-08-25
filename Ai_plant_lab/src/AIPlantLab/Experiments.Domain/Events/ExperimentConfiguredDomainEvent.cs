using Experiments.Domain.ValueObjects;

namespace Experiments.Domain.Events;

public sealed record ExperimentConfiguredDomainEvent(
    Guid ExperimentId,
    ExperimentConfiguration Configuration) : IDomainEvent;
