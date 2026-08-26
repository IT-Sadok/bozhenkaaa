using AIPlantLab.Contracts;
using Microsoft.Azure.Functions.Worker;
using Notifications.Functions.Constants;
using Notifications.Functions.Handlers;

namespace Notifications.Functions.Triggers.ServiceBus;

internal sealed class OnDiseaseDetected(DiseaseDetectedNotificationHandler handler)
{
    [Function(nameof(OnDiseaseDetected))]
    public async Task Run(
        [ServiceBusTrigger(MessagingEntityNames.DiseaseDetected, ServiceBusSubscriptionNames.Notifications, Connection = ConnectionStringNames.ServiceBus)]
        DiseaseDetectedIntegrationEvent message,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(message, cancellationToken);
    }
}
