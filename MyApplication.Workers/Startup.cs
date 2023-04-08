using MyApplication.Abstraction;
using MyApplication.Business;
using MyApplication.Workers.Workers;

namespace MyApplication.Workers;

public static class Startup
{
    public static IServiceCollection RegisterApp(this IServiceCollection services)
    {
        services.RegisterCore();
        services.RegisterBusiness();
        services.AddHostedService<AccountBackgroundWorker>();
        return services;
    }
}