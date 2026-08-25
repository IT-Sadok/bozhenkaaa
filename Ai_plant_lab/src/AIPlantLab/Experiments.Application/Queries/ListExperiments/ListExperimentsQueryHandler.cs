using Experiments.Application.Common;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Application.Mapping;
using Experiments.Application.Responses;
using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Queries.ListExperiments;

internal sealed class ListExperimentsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListExperimentsQuery, Result<PagedResult<ExperimentSummaryResponse>>>
{
    public async Task<Result<PagedResult<ExperimentSummaryResponse>>> Handle(
        ListExperimentsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page < 1)
        {
            return Result<PagedResult<ExperimentSummaryResponse>>.Failure("Page must be greater than or equal to 1.");
        }

        if (request.PageSize is < 1 or > 100)
        {
            return Result<PagedResult<ExperimentSummaryResponse>>.Failure("Page size must be between 1 and 100.");
        }

        var pagedExperiments = await unitOfWork.Experiments.ListAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        var pagedResult = new PagedResult<ExperimentSummaryResponse>
        {
            Items = pagedExperiments.Items.Select(ExperimentMapper.MapToSummaryResponse).ToList(),
            Page = pagedExperiments.Page,
            PageSize = pagedExperiments.PageSize,
            TotalCount = pagedExperiments.TotalCount
        };

        return Result<PagedResult<ExperimentSummaryResponse>>.Success(pagedResult);
    }
}
