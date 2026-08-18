namespace Experiments.Application.DTOs;

public sealed class ExperimentConfigurationDto
{
    public decimal LightHoursPerDay { get; init; }

    public int WateringIntervalDays { get; init; }

    public decimal TargetTemperatureCelsius { get; init; }

    public string? Notes { get; init; }
}
