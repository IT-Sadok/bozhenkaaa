using Notifications.Functions.Constants;

namespace Notifications.Functions.Channels;

public sealed class NotificationSettings
{
    public const string SectionName = NotificationConfigurationSections.Notifications;

    public EmailSettings Email { get; init; } = new();

    public TelegramSettings Telegram { get; init; } = new();
}
