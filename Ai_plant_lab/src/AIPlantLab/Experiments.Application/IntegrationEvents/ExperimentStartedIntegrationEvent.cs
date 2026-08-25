namespace Experiments.Application.IntegrationEvents;

public sealed record ExperimentStartedIntegrationEvent(
    Guid ExperimentId,
    DateTime StartedAt,
    DateTime OccurredAt);
