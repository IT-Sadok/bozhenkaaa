using AIAnalysis.Application.Interfaces.Messaging;
using AIAnalysis.Application.Interfaces.Repositories;
using AIAnalysis.Application.Interfaces.Services;
using AIAnalysis.Infrastructure.AI;
using AIAnalysis.Infrastructure.Messaging;
using AIAnalysis.Infrastructure.Persistence;
using AIAnalysis.Infrastructure.Persistence.Repositories;
using AIPlantLab.Contracts;
using Azure;
using Azure.AI.OpenAI;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AIAnalysis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringNames.DefaultConnection)
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringNames.DefaultConnection}' not found.");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseVector(); 
        var dataSource = dataSourceBuilder.Build();

        services.Configure<AzureOpenAiSettings>(configuration.GetSection(AzureOpenAiSettings.SectionName));
        
        services.AddScoped(provider =>
        {
            var settings = provider.GetRequiredService<IOptionsSnapshot<AzureOpenAiSettings>>().Value;

            if (string.IsNullOrWhiteSpace(settings.Endpoint))
                throw new InvalidOperationException("Azure OpenAI Endpoint is missing in configuration.");
                
            if (string.IsNullOrWhiteSpace(settings.ApiKey))
                throw new InvalidOperationException("Azure OpenAI ApiKey is missing in configuration.");
                         
            return new AzureOpenAIClient(new Uri(settings.Endpoint), new AzureKeyCredential(settings.ApiKey));
        });
        
        services.AddDbContext<AppDbContext>((_, options) =>
        {
            options.UseNpgsql(dataSource, npgsqlOptions =>
            {
                npgsqlOptions.UseVector();
            });
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

                    cfg.Message<DiseaseDetectedIntegrationEvent>(m => m.SetEntityName(MessagingEntityNames.DiseaseDetected));

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

                    cfg.Message<DiseaseDetectedIntegrationEvent>(m => m.SetEntityName(MessagingEntityNames.DiseaseDetected));

                    cfg.ConfigureEndpoints(context);
                });
            }
        });

        var useMockAi = configuration.GetValue<bool>("UseMockAi");
        if (useMockAi)
        {
            services.AddScoped<IAiVisionService, FakeAiVisionService>();
        }
        else
        {
            services.AddScoped<IAiVisionService, AzureOpenAiVisionService>();
        }
        
        services.AddScoped<IPlantDiagnosisRepository, PlantDiagnosisRepository>();
        services.AddScoped<IDiseaseRepository, DiseaseRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        return services;
    }
}
