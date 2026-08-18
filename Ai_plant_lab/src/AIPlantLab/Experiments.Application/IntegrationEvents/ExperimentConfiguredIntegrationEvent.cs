namespace Experiments.Application.IntegrationEvents;

public sealed record ExperimentConfiguredIntegrationEvent(
    Guid ExperimentId,
    decimal LightHoursPerDay,
    int WateringIntervalDays,
    decimal TargetTemperatureCelsius,
    string? Notes,
    DateTime OccurredAt);
