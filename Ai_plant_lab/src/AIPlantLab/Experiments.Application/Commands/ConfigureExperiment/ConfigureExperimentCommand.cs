using Experiments.Application.DTOs;
using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Commands.ConfigureExperiment;

public sealed record ConfigureExperimentCommand(
    Guid ExperimentId,
    ExperimentConfigurationDto Configuration) : IRequest<Result>;
