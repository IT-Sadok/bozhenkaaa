using Experiments.Application.Interfaces.Messaging;
using MassTransit;

namespace Experiments.Infrastructure.Messaging;

/// <summary>
/// Publishes integration events to RabbitMQ via MassTransit.
/// Must be called before SaveChanges (from domain event handlers) so the EF outbox
/// enlists messages in the same database transaction as domain state changes.
/// </summary>
public sealed class IntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : class
    {
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}
