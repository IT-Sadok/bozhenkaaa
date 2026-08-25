using Experiments.Application.Interfaces.Repositories;
using Experiments.Domain.Common;
using Experiments.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Experiments.Infrastructure.Persistence;

public sealed class UnitOfWork(
    AppDbContext dbContext,
    IExperimentRepository experiments,
    IMediator mediator) : IUnitOfWork
{
    public IExperimentRepository Experiments { get; } = experiments;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = CollectDomainEvents();

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        var rowsAffected = await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return rowsAffected;
    }

    private List<IDomainEvent> CollectDomainEvents()
    {
        return dbContext.ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .SelectMany(e =>
            {
                var events = e.DomainEvents.ToList();
                e.ClearDomainEvents();
                return events;
            })
            .ToList();
    }
}
