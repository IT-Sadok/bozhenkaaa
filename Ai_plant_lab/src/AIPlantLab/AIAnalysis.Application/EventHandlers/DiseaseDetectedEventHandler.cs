using AIAnalysis.Application.Interfaces;
using AIAnalysis.Application.Interfaces.Messaging;
using AIAnalysis.Domain.Events;
using AIPlantLab.Contracts;
using MediatR;

namespace AIAnalysis.Application.EventHandlers;

internal sealed class DiseaseDetectedEventHandler(
    IIntegrationEventPublisher publisher,
    IDateTimeProvider dateTime)
    : INotificationHandler<DiseaseDetectedEvent>
{
    public async Task Handle(DiseaseDetectedEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new DiseaseDetectedIntegrationEvent(
            notification.DiagnosisId,
            notification.ExperimentId,
            notification.DetectedDiseaseId,
            notification.DiseaseName,
            notification.Recommendations,
            notification.IsContagious,
            notification.LethalityIndex,
            dateTime.UtcNow);

        await publisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
