using Experiments.Domain.Common;
using Experiments.Domain.Entities;
using Experiments.Domain.ValueObjects;

namespace Experiments.Domain.Services;

public interface IExperimentService
{
    Result<Experiment> Create(string name, string? description);

    Result Configure(Experiment experiment, ExperimentConfiguration configuration);

    Result<PlantGroup> AddPlantGroup(Experiment experiment, string name, string species, int plantCount);

    Result Start(Experiment experiment);

    Result Finish(Experiment experiment);
}
