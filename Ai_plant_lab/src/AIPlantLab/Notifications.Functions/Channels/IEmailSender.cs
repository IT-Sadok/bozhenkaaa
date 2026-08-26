namespace Notifications.Functions.Channels;

public interface IEmailSender
{
    Task SendAsync(string subject, string body, CancellationToken cancellationToken = default);
}
