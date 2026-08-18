using System.Reflection;
using Experiments.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Experiments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddScoped<IExperimentService, ExperimentService>();

        return services;
    }
}
