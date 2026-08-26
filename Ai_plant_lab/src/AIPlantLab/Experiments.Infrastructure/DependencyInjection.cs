using AIPlantLab.Contracts;
using Experiments.Application.Interfaces.Messaging;
using Experiments.Application.Interfaces.Repositories;
using Experiments.Infrastructure.Messaging;
using Experiments.Infrastructure.Persistence;
using Experiments.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Experiments.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringNames.DefaultConnection)
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringNames.DefaultConnection}' not found.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        var transport = configuration[$"{ConfigurationSections.Messaging}:Transport"]
            ?? MessagingTransports.RabbitMq;

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
                o.QueryDelay = TimeSpan.FromSeconds(10);
            });

            if (string.Equals(transport, MessagingTransports.ServiceBus, StringComparison.OrdinalIgnoreCase))
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(configuration.GetConnectionString(ConnectionStringNames.ServiceBus)
                        ?? throw new InvalidOperationException($"Connection string '{ConnectionStringNames.ServiceBus}' not found."));

                    cfg.UseRawJsonSerializer();

                    cfg.Message<ExperimentFinishedIntegrationEvent>(m => m.SetEntityName(MessagingEntityNames.ExperimentFinished));

                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                var rabbitMqSettings = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>()
                    ?? throw new InvalidOperationException($"Configuration section '{RabbitMqSettings.SectionName}' is missing.");

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });

                    cfg.Message<ExperimentFinishedIntegrationEvent>(m => m.SetEntityName(MessagingEntityNames.ExperimentFinished));

                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        services.AddScoped<IExperimentRepository, ExperimentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        return services;
    }
}
