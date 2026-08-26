using AIPlantLab.Contracts;
using MassTransit;
using Notifications.Functions.Handlers;

namespace Notifications.Functions.Triggers.RabbitMq;

internal sealed class ExperimentFinishedConsumer(
    ExperimentFinishedNotificationHandler handler) : IConsumer<ExperimentFinishedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ExperimentFinishedIntegrationEvent> context)
    {
        await handler.HandleAsync(context.Message, context.CancellationToken);
    }
}
