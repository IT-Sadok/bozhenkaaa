namespace Experiments.Application.IntegrationEvents;

public sealed record ExperimentFinishedIntegrationEvent(
    Guid ExperimentId,
    DateTime FinishedAt,
    DateTime OccurredAt);
