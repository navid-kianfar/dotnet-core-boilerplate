using Microsoft.Extensions.DependencyInjection;
using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Implementation;

namespace MyApplication.Abstraction;

public static class Startup
{
    public static IServiceCollection RegisterCore(this IServiceCollection services)
    {
        services.AddSingleton<IJsonService, JsonService>();
        services.AddSingleton<ILoggerService, ConsoleLogger>();
        return services;
    }
}