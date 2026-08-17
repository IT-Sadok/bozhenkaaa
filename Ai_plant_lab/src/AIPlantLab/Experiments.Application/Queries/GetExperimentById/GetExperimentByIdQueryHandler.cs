using AutoMapper;
using Experiments.Application.DTOs;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Queries.GetExperimentById;

internal sealed class GetExperimentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetExperimentByIdQuery, Result<ExperimentDto>>
{
    public async Task<Result<ExperimentDto>> Handle(GetExperimentByIdQuery request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.Id, cancellationToken);
        return experiment is null
            ? Result<ExperimentDto>.ErrorResult("Experiment not found.")
            : Result<ExperimentDto>.Success(mapper.Map<ExperimentDto>(experiment));
    }
}
