namespace Experiments.Application.Responses;

public sealed class ExperimentConfigurationResponse
{
    public decimal LightHoursPerDay { get; init; }

    public int WateringIntervalDays { get; init; }

    public decimal TargetTemperatureCelsius { get; init; }

    public string? Notes { get; init; }
}
