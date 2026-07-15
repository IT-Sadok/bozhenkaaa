using AIAnalysis.Application.Interfaces;
using AIAnalysis.Application.Interfaces.Repositories;
using AIAnalysis.Application.Interfaces.Services;
using AIAnalysis.Infrastructure.AI;
using AIAnalysis.Infrastructure.Persistence;
using AIAnalysis.Infrastructure.Persistence.Repositories;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AIAnalysis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.Configure<AzureOpenAiSettings>(configuration.GetSection(AzureOpenAiSettings.SectionName));
        
        services.AddSingleton(provider =>
        {
            var settings = provider.GetRequiredService<IOptionsSnapshot<AzureOpenAiSettings>>().Value;

            if (string.IsNullOrWhiteSpace(settings.Endpoint))
                throw new InvalidOperationException("Azure OpenAI Endpoint is missing in configuration.");
                
            if (string.IsNullOrWhiteSpace(settings.ApiKey))
                throw new InvalidOperationException("Azure OpenAI ApiKey is missing in configuration.");
                         
            return new AzureOpenAIClient(new Uri(settings.Endpoint), new AzureKeyCredential(settings.ApiKey));
        });

        services.AddScoped<IAiVisionService, AzureOpenAiVisionService>();
        
        services.AddScoped<IPlantDiagnosisRepository, PlantDiagnosisRepository>();
        services.AddScoped<IDiseaseRepository, DiseaseRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}