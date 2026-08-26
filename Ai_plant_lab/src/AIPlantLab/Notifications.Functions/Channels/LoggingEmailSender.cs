using Microsoft.Extensions.Logging;

namespace Notifications.Functions.Channels;

internal sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string subject, string body, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Email notification (not sent) — Subject: {Subject}, Body: {Body}", subject, body);
        return Task.CompletedTask;
    }
}
