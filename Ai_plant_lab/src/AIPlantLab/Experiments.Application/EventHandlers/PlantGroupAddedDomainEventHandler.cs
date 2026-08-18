using Experiments.Application.IntegrationEvents;
using Experiments.Application.Interfaces;
using Experiments.Application.Interfaces.Messaging;
using Experiments.Domain.Events;
using MediatR;

namespace Experiments.Application.EventHandlers;

internal sealed class PlantGroupAddedDomainEventHandler(
    IIntegrationEventPublisher publisher,
    IDateTimeProvider dateTime)
    : INotificationHandler<PlantGroupAddedDomainEvent>
{
    public async Task Handle(PlantGroupAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new PlantGroupAddedIntegrationEvent(
            notification.ExperimentId,
            notification.PlantGroupId,
            notification.Name,
            notification.Species,
            notification.PlantCount,
            dateTime.UtcNow);

        await publisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
