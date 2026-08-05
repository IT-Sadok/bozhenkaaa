using AIAnalysis.Application.Interfaces.Repositories;
using AIAnalysis.Domain.Common;
using MassTransit;

namespace AIAnalysis.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public IPlantDiagnosisRepository Diagnoses { get; }
    public IDiseaseRepository Diseases { get; }

    public UnitOfWork(
        AppDbContext dbContext,
        IPlantDiagnosisRepository diagnoses,
        IDiseaseRepository diseases, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        Diagnoses = diagnoses;
        Diseases = diseases;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = _dbContext.ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .SelectMany(e => {
                var events = e.DomainEvents.ToList();
                e.ClearDomainEvents();
                return events;
            })
            .ToList();
        
        foreach (var domainEvent in domainEvents)
        {
            await _publishEndpoint.Publish(domainEvent, cancellationToken);
        }
        
        var result = await _dbContext.SaveChangesAsync(cancellationToken);
        
        return result;
    }
}