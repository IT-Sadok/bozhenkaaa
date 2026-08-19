using Experiments.Application.Interfaces;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Entities;
using Experiments.Domain.Enums;
using Experiments.Domain.Events;
using FluentValidation;
using MediatR;

namespace Experiments.Application.Commands.CreateExperiment;

internal sealed class CreateExperimentCommandHandler(
    IValidator<CreateExperimentCommand> validator,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTime)
    : IRequestHandler<CreateExperimentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateExperimentCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<Guid>.Failure(validation.Errors.Select(e => e.ErrorMessage));
        }

        var experiment = new Experiment
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Status = ExperimentStatus.Draft,
            CreatedAt = dateTime.UtcNow
        };

        experiment.RaiseDomainEvent(new ExperimentCreatedDomainEvent(
            experiment.Id,
            experiment.Name,
            experiment.Description));

        unitOfWork.Experiments.Add(experiment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(experiment.Id);
    }
}
