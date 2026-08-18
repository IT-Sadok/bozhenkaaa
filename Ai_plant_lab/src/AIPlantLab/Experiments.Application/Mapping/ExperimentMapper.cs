using Experiments.Application.DTOs;
using Experiments.Domain.Entities;
using Experiments.Domain.ValueObjects;

namespace Experiments.Application.Mapping;

internal static class ExperimentMapper
{
    public static ExperimentDto MapToDto(Experiment experiment)
    {
        return new ExperimentDto
        {
            Id = experiment.Id,
            Name = experiment.Name,
            Description = experiment.Description,
            Status = experiment.Status.ToString(),
            Configuration = experiment.Configuration is null
                ? null
                : MapToConfigurationDto(experiment.Configuration),
            CreatedAt = experiment.CreatedAt,
            StartedAt = experiment.StartedAt,
            FinishedAt = experiment.FinishedAt,
            PlantGroups = experiment.PlantGroups.Select(MapToPlantGroupDto).ToList()
        };
    }

    public static ExperimentSummaryDto MapToSummaryDto(Experiment experiment)
    {
        return new ExperimentSummaryDto
        {
            Id = experiment.Id,
            Name = experiment.Name,
            Status = experiment.Status.ToString(),
            PlantGroupCount = experiment.PlantGroups.Count,
            CreatedAt = experiment.CreatedAt
        };
    }

    private static ExperimentConfigurationDto MapToConfigurationDto(ExperimentConfiguration configuration)
    {
        return new ExperimentConfigurationDto
        {
            LightHoursPerDay = configuration.LightHoursPerDay,
            WateringIntervalDays = configuration.WateringIntervalDays,
            TargetTemperatureCelsius = configuration.TargetTemperatureCelsius,
            Notes = configuration.Notes
        };
    }

    private static PlantGroupDto MapToPlantGroupDto(PlantGroup plantGroup)
    {
        return new PlantGroupDto
        {
            Id = plantGroup.Id,
            Name = plantGroup.Name,
            Species = plantGroup.Species,
            PlantCount = plantGroup.PlantCount
        };
    }
}
