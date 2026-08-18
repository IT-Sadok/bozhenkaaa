using Experiments.Application.IntegrationEvents;
using Experiments.Application.Interfaces;
using Experiments.Application.Interfaces.Messaging;
using Experiments.Domain.Events;
using MediatR;

namespace Experiments.Application.EventHandlers;

internal sealed class ExperimentConfiguredDomainEventHandler(
    IIntegrationEventPublisher publisher,
    IDateTimeProvider dateTime)
    : INotificationHandler<ExperimentConfiguredDomainEvent>
{
    public async Task Handle(ExperimentConfiguredDomainEvent notification, CancellationToken cancellationToken)
    {
        var config = notification.Configuration;

        var integrationEvent = new ExperimentConfiguredIntegrationEvent(
            notification.ExperimentId,
            config.LightHoursPerDay,
            config.WateringIntervalDays,
            config.TargetTemperatureCelsius,
            config.Notes,
            dateTime.UtcNow);

        await publisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
