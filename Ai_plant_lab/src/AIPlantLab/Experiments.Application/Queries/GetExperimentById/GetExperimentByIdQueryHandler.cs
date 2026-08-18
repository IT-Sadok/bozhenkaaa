using Experiments.Application.DTOs;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Application.Mapping;
using Experiments.Domain.Common;
using Experiments.Domain.Constants;
using Experiments.Domain.Entities;
using MediatR;

namespace Experiments.Application.Queries.GetExperimentById;

internal sealed class GetExperimentByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExperimentByIdQuery, Result<ExperimentDto>>
{
    public async Task<Result<ExperimentDto>> Handle(GetExperimentByIdQuery request, CancellationToken cancellationToken)
    {
        var experiment = await unitOfWork.Experiments.GetByIdAsync(request.Id, cancellationToken);
        return experiment is null
            ? Result<ExperimentDto>.Failure(ValidationMessages.NotFound, source: nameof(Experiment))
            : Result<ExperimentDto>.Success(ExperimentMapper.MapToDto(experiment));
    }
}
