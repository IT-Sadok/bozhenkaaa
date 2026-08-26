using AIPlantLab.Contracts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Functions.Channels;
using Notifications.Functions.Constants;
using Notifications.Functions.Handlers;
using Notifications.Functions.Triggers.RabbitMq;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.Configure<NotificationSettings>(
            context.Configuration.GetSection(NotificationConfigurationSections.Notifications));
        services.Configure<RabbitMqSettings>(
            context.Configuration.GetSection(ConfigurationSections.RabbitMq));

        services.AddHttpClient();
        services.AddSingleton<IEmailSender, LoggingEmailSender>();
        services.AddSingleton<ITelegramSender, TelegramBotSender>();
        services.AddScoped<ExperimentFinishedNotificationHandler>();
        services.AddScoped<DiseaseDetectedNotificationHandler>();

        var transport = context.Configuration[$"{ConfigurationSections.Messaging}:Transport"]
            ?? context.Configuration["Messaging__Transport"]
            ?? MessagingTransports.RabbitMq;

        if (string.Equals(transport, MessagingTransports.ServiceBus, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var rabbitMqSettings = context.Configuration.GetSection(ConfigurationSections.RabbitMq).Get<RabbitMqSettings>()
            ?? new RabbitMqSettings();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ExperimentFinishedConsumer>();
            x.AddConsumer<DiseaseDetectedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.Message<ExperimentFinishedIntegrationEvent>(m => m.SetEntityName(MessagingEntityNames.ExperimentFinished));
                cfg.Message<DiseaseDetectedIntegrationEvent>(m => m.SetEntityName(MessagingEntityNames.DiseaseDetected));

                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

host.Run();
