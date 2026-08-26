namespace AIPlantLab.Contracts;

public sealed record ExperimentFinishedIntegrationEvent(
    Guid ExperimentId,
    string Name,
    DateTime FinishedAt,
    DateTime OccurredAt);
