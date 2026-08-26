using System.Reflection;
using AIAnalysis.Application.Common;
using AIAnalysis.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AIAnalysis.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}