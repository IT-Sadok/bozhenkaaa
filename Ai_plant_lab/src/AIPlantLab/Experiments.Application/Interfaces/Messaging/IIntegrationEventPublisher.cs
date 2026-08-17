namespace Experiments.Application.Interfaces.Messaging;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : class;
}
