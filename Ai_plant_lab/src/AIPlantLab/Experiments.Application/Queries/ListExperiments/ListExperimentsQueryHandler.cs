using AutoMapper;
using Experiments.Application.DTOs;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Queries.ListExperiments;

internal sealed class ListExperimentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<ListExperimentsQuery, Result<PagedResult<ExperimentSummaryDto>>>
{
    public async Task<Result<PagedResult<ExperimentSummaryDto>>> Handle(
        ListExperimentsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page < 1)
        {
            return Result<PagedResult<ExperimentSummaryDto>>.ErrorResult("Page must be greater than or equal to 1.");
        }

        if (request.PageSize is < 1 or > 100)
        {
            return Result<PagedResult<ExperimentSummaryDto>>.ErrorResult("Page size must be between 1 and 100.");
        }

        var (items, totalCount) = await unitOfWork.Experiments.ListAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        var summaries = items
            .Select(mapper.Map<ExperimentSummaryDto>)
            .ToList();

        var pagedResult = new PagedResult<ExperimentSummaryDto>
        {
            Items = summaries,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<ExperimentSummaryDto>>.Success(pagedResult);
    }
}
