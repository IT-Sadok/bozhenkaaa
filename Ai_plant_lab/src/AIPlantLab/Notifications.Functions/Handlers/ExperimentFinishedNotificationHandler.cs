using AIPlantLab.Contracts;
using Microsoft.Extensions.Logging;
using Notifications.Functions.Channels;
using Notifications.Functions.Constants;

namespace Notifications.Functions.Handlers;

internal sealed class ExperimentFinishedNotificationHandler(
    IEmailSender emailSender,
    ITelegramSender telegramSender,
    ILogger<ExperimentFinishedNotificationHandler> logger)
{
    public async Task HandleAsync(ExperimentFinishedIntegrationEvent message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Sending notifications for finished experiment {ExperimentId}",
            message.ExperimentId);

        var subject = string.Format(ExperimentFinishedNotificationTemplates.SubjectFormat, message.Name);
        var body = string.Format(
            ExperimentFinishedNotificationTemplates.BodyFormat,
            message.ExperimentId,
            message.FinishedAt);

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
