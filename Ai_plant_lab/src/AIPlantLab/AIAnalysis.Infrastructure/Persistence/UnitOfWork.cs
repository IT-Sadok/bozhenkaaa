using AIAnalysis.Application.Interfaces.Repositories;
using AIAnalysis.Domain.Common;
using AIAnalysis.Domain.Events;
using MediatR;

namespace AIAnalysis.Infrastructure.Persistence;

public sealed class UnitOfWork(
    AppDbContext dbContext,
    IPlantDiagnosisRepository diagnoses,
    IDiseaseRepository diseases,
    IMediator mediator) : IUnitOfWork
{
    public IPlantDiagnosisRepository Diagnoses { get; } = diagnoses;

    public IDiseaseRepository Diseases { get; } = diseases;

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
