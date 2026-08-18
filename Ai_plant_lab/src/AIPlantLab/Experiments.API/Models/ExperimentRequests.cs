namespace Experiments.API.Models;

public sealed record CreateExperimentRequest(string Name, string? Description);

public sealed record ConfigureExperimentRequest(
    decimal LightHoursPerDay,
    int WateringIntervalDays,
    decimal TargetTemperatureCelsius,
    string? Notes);

public sealed record AddPlantGroupRequest(string Name, string Species, int PlantCount);
