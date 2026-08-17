namespace Experiments.Application.IntegrationEvents;

public sealed record ExperimentConfiguredIntegrationEvent(
    Guid ExperimentId,
    int LightHoursPerDay,
    int WateringIntervalDays,
    decimal TargetTemperatureCelsius,
    string? Notes,
    DateTime OccurredAt);
