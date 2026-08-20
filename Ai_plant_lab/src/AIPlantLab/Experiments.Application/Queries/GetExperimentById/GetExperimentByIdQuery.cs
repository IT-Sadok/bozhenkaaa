using Experiments.Application.Responses;
using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Queries.GetExperimentById;

public sealed record GetExperimentByIdQuery(Guid Id) : IRequest<Result<ExperimentResponse>>;
