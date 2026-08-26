using AIAnalysis.Application.Interfaces.Messaging;
using MassTransit;

namespace AIAnalysis.Infrastructure.Messaging;

public sealed class IntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : class
    {
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}
