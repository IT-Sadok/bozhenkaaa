using Experiments.Domain.Events;

namespace Experiments.Domain.Common;

/// <summary>
/// Base type for domain entities and aggregates.
/// Aggregates collect domain events via <see cref="AddDomainEvent"/> during business operations;
/// the infrastructure layer dispatches them through MediatR after SaveChanges is initiated.
/// Domain events are internal to this bounded context and are never published directly to RabbitMQ.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
