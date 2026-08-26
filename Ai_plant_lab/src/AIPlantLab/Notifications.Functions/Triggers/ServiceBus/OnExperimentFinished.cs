using AIPlantLab.Contracts;
using Microsoft.Azure.Functions.Worker;
using Notifications.Functions.Constants;
using Notifications.Functions.Handlers;

namespace Notifications.Functions.Triggers.ServiceBus;

internal sealed class OnExperimentFinished(ExperimentFinishedNotificationHandler handler)
{
    [Function(nameof(OnExperimentFinished))]
    public async Task Run(
        [ServiceBusTrigger(MessagingEntityNames.ExperimentFinished, ServiceBusSubscriptionNames.Notifications, Connection = ConnectionStringNames.ServiceBus)]
        ExperimentFinishedIntegrationEvent message,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(message, cancellationToken);
    }
}
