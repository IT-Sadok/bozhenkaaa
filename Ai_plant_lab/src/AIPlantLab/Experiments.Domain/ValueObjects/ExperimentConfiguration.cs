namespace Experiments.Domain.ValueObjects;

public sealed record ExperimentConfiguration(
    int LightHoursPerDay,
    int WateringIntervalDays,
    decimal TargetTemperatureCelsius,
    string? Notes);
