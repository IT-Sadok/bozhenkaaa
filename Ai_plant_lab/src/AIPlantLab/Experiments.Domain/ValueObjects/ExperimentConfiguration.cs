namespace Experiments.Domain.ValueObjects;

public sealed record ExperimentConfiguration(
    decimal LightHoursPerDay,
    int WateringIntervalDays,
    decimal TargetTemperatureCelsius,
    string? Notes);
