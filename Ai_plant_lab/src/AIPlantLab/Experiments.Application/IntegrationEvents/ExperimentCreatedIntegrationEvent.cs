namespace Experiments.Application.IntegrationEvents;

public sealed record ExperimentCreatedIntegrationEvent(
    Guid ExperimentId,
    string Name,
    string? Description,
    DateTime OccurredAt);
