using AIPlantLab.Contracts;
using Microsoft.Extensions.Logging;
using Notifications.Functions.Channels;
using Notifications.Functions.Constants;

namespace Notifications.Functions.Handlers;

internal sealed class DiseaseDetectedNotificationHandler(
    IEmailSender emailSender,
    ITelegramSender telegramSender,
    ILogger<DiseaseDetectedNotificationHandler> logger)
{
    public async Task HandleAsync(DiseaseDetectedIntegrationEvent message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Sending notifications for disease detected on experiment {ExperimentId}",
            message.ExperimentId);

        var subject = string.Format(DiseaseDetectedNotificationTemplates.SubjectFormat, message.DiseaseName);
        var body = string.Format(
            DiseaseDetectedNotificationTemplates.BodyFormat,
            message.ExperimentId,
            message.IsContagious,
            message.LethalityIndex,
            message.Recommendations);

        await Task.WhenAll(
            SendTelegramAsync($"{subject}{Environment.NewLine}{body}", message.ExperimentId, cancellationToken),
            SendEmailAsync(subject, body, message.ExperimentId, cancellationToken));
    }

    private async Task SendTelegramAsync(string text, Guid experimentId, CancellationToken cancellationToken)
    {
        try
        {
            await telegramSender.SendAsync(text, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send Telegram notification for experiment {ExperimentId}", experimentId);
        }
    }

    private async Task SendEmailAsync(string subject, string body, Guid experimentId, CancellationToken cancellationToken)
    {
        try
        {
            await emailSender.SendAsync(subject, body, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email notification for experiment {ExperimentId}", experimentId);
        }
    }
}
