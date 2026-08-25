using Experiments.Application.IntegrationEvents;
using Experiments.Application.Interfaces;
using Experiments.Application.Interfaces.Messaging;
using Experiments.Domain.Events;
using MediatR;

namespace Experiments.Application.EventHandlers;

internal sealed class ExperimentFinishedDomainEventHandler(
    IIntegrationEventPublisher publisher,
    IDateTimeProvider dateTime)
    : INotificationHandler<ExperimentFinishedDomainEvent>
{
    public async Task Handle(ExperimentFinishedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new ExperimentFinishedIntegrationEvent(
            notification.ExperimentId,
            notification.FinishedAt,
            dateTime.UtcNow);

        await publisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
