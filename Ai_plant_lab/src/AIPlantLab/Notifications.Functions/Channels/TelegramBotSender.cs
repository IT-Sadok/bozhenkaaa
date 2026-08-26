using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifications.Functions.Constants;

namespace Notifications.Functions.Channels;

internal sealed class TelegramBotSender(
    IHttpClientFactory httpClientFactory,
    IOptions<NotificationSettings> settings,
    ILogger<TelegramBotSender> logger) : ITelegramSender
{
    public async Task SendAsync(string text, CancellationToken cancellationToken = default)
    {
        var telegram = settings.Value.Telegram;

        if (string.IsNullOrEmpty(telegram.BotToken) || string.IsNullOrEmpty(telegram.ChatId))
        {
            logger.LogWarning("Telegram BotToken or ChatId not configured; skipping Telegram notification.");
            return;
        }

        var client = httpClientFactory.CreateClient();
        var url = string.Format(TelegramApiConstants.SendMessageUrlFormat, telegram.BotToken);
        var payload = new Dictionary<string, string>
        {
            [TelegramApiConstants.ChatIdProperty] = telegram.ChatId,
            [TelegramApiConstants.TextProperty] = text
        };

        var response = await client.PostAsJsonAsync(url, payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Telegram API returned {StatusCode}: {Content}", response.StatusCode, content);
            throw new HttpRequestException($"Telegram API failed with status {response.StatusCode}");
        }
    }
}
