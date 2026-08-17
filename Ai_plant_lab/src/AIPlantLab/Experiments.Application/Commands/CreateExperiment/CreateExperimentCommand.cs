using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Commands.CreateExperiment;

public sealed record CreateExperimentCommand(string Name, string? Description) : IRequest<Result<Guid>>;
