using AIAnalysis.Application.Interfaces.Repositories;
using AIAnalysis.Application.Interfaces.Services;
using AIAnalysis.Infrastructure.AI;
using AIAnalysis.Infrastructure.Persistence;
using AIAnalysis.Infrastructure.Persistence.Interceptors;
using AIAnalysis.Infrastructure.Persistence.Repositories;
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
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

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
            options.AddInterceptors(new DomainEventsPublishInterceptor());
        });

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();

                o.QueryDelay = TimeSpan.FromSeconds(10);
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
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

        return services;
    }
}