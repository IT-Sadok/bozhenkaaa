namespace Notifications.Functions.Channels;

public sealed class TelegramSettings
{
    public string BotToken { get; init; } = "";

    public string ChatId { get; init; } = "";
}
