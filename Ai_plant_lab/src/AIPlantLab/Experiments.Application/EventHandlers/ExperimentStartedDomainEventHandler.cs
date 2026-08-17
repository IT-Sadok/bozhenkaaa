using Experiments.Application.IntegrationEvents;
using Experiments.Application.Interfaces.Messaging;
using Experiments.Domain.Events;
using MediatR;

namespace Experiments.Application.EventHandlers;

internal sealed class ExperimentStartedDomainEventHandler(IIntegrationEventPublisher publisher)
    : INotificationHandler<ExperimentStartedDomainEvent>
{
    public async Task Handle(ExperimentStartedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ExperimentStartedIntegrationEvent(
            notification.ExperimentId,
            notification.StartedAt,
            DateTime.UtcNow);

        await publisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
