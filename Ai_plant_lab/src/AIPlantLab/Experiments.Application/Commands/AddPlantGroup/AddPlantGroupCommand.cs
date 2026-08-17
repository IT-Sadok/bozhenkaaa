using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Commands.AddPlantGroup;

public sealed record AddPlantGroupCommand(
    Guid ExperimentId,
    string Name,
    string Species,
    int PlantCount) : IRequest<Result<Guid>>;
