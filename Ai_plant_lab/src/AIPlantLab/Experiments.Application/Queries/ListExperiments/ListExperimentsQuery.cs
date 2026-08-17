using Experiments.Application.DTOs;
using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Queries.ListExperiments;

public sealed record ListExperimentsQuery(int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<ExperimentSummaryDto>>>;
