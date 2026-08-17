using Experiments.Application.IntegrationEvents;
using Experiments.Application.Interfaces.Messaging;
using Experiments.Domain.Events;
using MediatR;

namespace Experiments.Application.EventHandlers;

internal sealed class ExperimentCreatedDomainEventHandler(IIntegrationEventPublisher publisher)
    : INotificationHandler<ExperimentCreatedDomainEvent>
{
    public async Task Handle(ExperimentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ExperimentCreatedIntegrationEvent(
            notification.ExperimentId,
            notification.Name,
            notification.Description,
            DateTime.UtcNow);

        await publisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
