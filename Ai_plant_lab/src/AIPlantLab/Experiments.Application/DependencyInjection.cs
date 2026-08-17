using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Experiments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddAutoMapper(_ => { }, Assembly.GetExecutingAssembly());

        return services;
    }
}
