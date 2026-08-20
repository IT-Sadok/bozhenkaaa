using Experiments.Application.Interfaces.Repositories;
using Experiments.Application.Mapping;
using Experiments.Application.Responses;
using Experiments.Domain.Common;
using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using MediatR;

namespace Experiments.Application.Queries.GetExperimentById;

internal sealed class GetExperimentByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExperimentByIdQuery, Result<ExperimentResponse>>
{
    public async Task<Result<ExperimentResponse>> Handle(GetExperimentByIdQuery request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.Id, cancellationToken);
        return experiment is null
            ? Result<ExperimentResponse>.Failure(ValidationMessages.NotFound, source: nameof(Experiment))
            : Result<ExperimentResponse>.Success(ExperimentMapper.MapToResponse(experiment));
    }
}
