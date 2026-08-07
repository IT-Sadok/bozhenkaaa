using AIAnalysis.Domain.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AIAnalysis.Infrastructure.Persistence.Interceptors;

public class DomainEventsPublishInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var domainEvents = dbContext.ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .SelectMany(e => {
                var events = e.DomainEvents.ToList();
                e.ClearDomainEvents();
                return events;
            })
            .ToList();

        if (domainEvents.Count != 0)
        {
            var publishEndpoint = dbContext.GetService<IPublishEndpoint>();

            foreach (var domainEvent in domainEvents)
            {
                await publishEndpoint.Publish(domainEvent, cancellationToken);
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}