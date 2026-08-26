using AIPlantLab.Contracts;

namespace AIAnalysis.Infrastructure.Messaging;

public sealed class RabbitMqSettings
{
    public const string SectionName = ConfigurationSections.RabbitMq;

    public string Host { get; init; } = "localhost";

    public string VirtualHost { get; init; } = "/";

    public string Username { get; init; } = "guest";

    public string Password { get; init; } = "guest";
}
