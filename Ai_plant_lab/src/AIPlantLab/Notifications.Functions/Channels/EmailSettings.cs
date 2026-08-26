namespace Notifications.Functions.Channels;

public sealed class EmailSettings
{
    public string Host { get; init; } = "";

    public int Port { get; init; } = 2525;

    public string From { get; init; } = "";

    public string To { get; init; } = "you@example.com";

    public string? Username { get; init; }

    public string? Password { get; init; }
}
