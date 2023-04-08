using Microsoft.Extensions.DependencyInjection;
using MyApplication.Abstraction.Contracts;
using MyApplication.Business.Account;
using MyApplication.Business.Core;
using MyApplication.DataAccess;

namespace MyApplication.Business;

public static class Startup
{
    public static IServiceCollection RegisterBusiness(this IServiceCollection services)
    {
        services.RegisterDataAccess();
        services.AddScoped<IAccountService, AccountService>();
        services.AddSingleton<IQueueService, QueueService>();
        return services;
    }
}