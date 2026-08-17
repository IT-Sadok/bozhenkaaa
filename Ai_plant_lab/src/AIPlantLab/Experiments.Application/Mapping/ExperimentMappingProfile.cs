using AutoMapper;
using Experiments.Application.DTOs;
using Experiments.Domain.Entities;
using Experiments.Domain.ValueObjects;

namespace Experiments.Application.Mapping;

internal sealed class ExperimentMappingProfile : Profile
{
    public ExperimentMappingProfile()
    {
        CreateMap<PlantGroup, PlantGroupDto>();
        CreateMap<ExperimentConfiguration, ExperimentConfigurationDto>();

        CreateMap<Experiment, ExperimentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PlantGroups, opt => opt.MapFrom(src => src.PlantGroups));

        CreateMap<Experiment, ExperimentSummaryDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PlantGroupCount, opt => opt.MapFrom(src => src.PlantGroups.Count));
    }
}
