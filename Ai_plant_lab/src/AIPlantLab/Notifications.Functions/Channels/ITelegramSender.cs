namespace Notifications.Functions.Channels;

public interface ITelegramSender
{
    Task SendAsync(string text, CancellationToken cancellationToken = default);
}
