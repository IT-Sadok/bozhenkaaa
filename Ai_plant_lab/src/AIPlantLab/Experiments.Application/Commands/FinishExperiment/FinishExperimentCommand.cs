using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Commands.FinishExperiment;

public sealed record FinishExperimentCommand(Guid ExperimentId) : IRequest<Result>;
