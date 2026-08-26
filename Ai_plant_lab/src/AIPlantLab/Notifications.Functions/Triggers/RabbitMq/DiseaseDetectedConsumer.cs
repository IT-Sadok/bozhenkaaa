using AIPlantLab.Contracts;
using MassTransit;
using Notifications.Functions.Handlers;

namespace Notifications.Functions.Triggers.RabbitMq;

internal sealed class DiseaseDetectedConsumer(
    DiseaseDetectedNotificationHandler handler) : IConsumer<DiseaseDetectedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<DiseaseDetectedIntegrationEvent> context)
    {
        await handler.HandleAsync(context.Message, context.CancellationToken);
    }
}
