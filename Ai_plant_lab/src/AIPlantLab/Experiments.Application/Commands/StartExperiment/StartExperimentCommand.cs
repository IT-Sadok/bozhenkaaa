using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Commands.StartExperiment;

public sealed record StartExperimentCommand(Guid ExperimentId) : IRequest<Result>;
