using Experiments.Application.Responses;
using Experiments.Domain.Entities;
using Experiments.Domain.ValueObjects;

namespace Experiments.Application.Mapping;

internal static class ExperimentMapper
{
    public static ExperimentResponse MapToResponse(Experiment experiment)
    {
        return new ExperimentResponse
        {
            Id = experiment.Id,
            Name = experiment.Name,
            Description = experiment.Description,
            Status = experiment.Status.ToString(),
            Configuration = experiment.Configuration is null
                ? null
                : MapToConfigurationResponse(experiment.Configuration),
            CreatedAt = experiment.CreatedAt,
            StartedAt = experiment.StartedAt,
            FinishedAt = experiment.FinishedAt,
            PlantGroups = experiment.PlantGroups.Select(MapToPlantGroupResponse).ToList()
        };
    }

    public static ExperimentSummaryResponse MapToSummaryResponse(Experiment experiment)
    {
        return new ExperimentSummaryResponse
        {
            Id = experiment.Id,
            Name = experiment.Name,
            Status = experiment.Status.ToString(),
            PlantGroupCount = experiment.PlantGroups.Count,
            CreatedAt = experiment.CreatedAt
        };
    }

    private static ExperimentConfigurationResponse MapToConfigurationResponse(ExperimentConfiguration configuration)
    {
        return new ExperimentConfigurationResponse
        {
            LightHoursPerDay = configuration.LightHoursPerDay,
            WateringIntervalDays = configuration.WateringIntervalDays,
            TargetTemperatureCelsius = configuration.TargetTemperatureCelsius,
            Notes = configuration.Notes
        };
    }

    private static PlantGroupResponse MapToPlantGroupResponse(PlantGroup plantGroup)
    {
        return new PlantGroupResponse
        {
            Id = plantGroup.Id,
            Name = plantGroup.Name,
            Species = plantGroup.Species,
            PlantCount = plantGroup.PlantCount
        };
    }
}
