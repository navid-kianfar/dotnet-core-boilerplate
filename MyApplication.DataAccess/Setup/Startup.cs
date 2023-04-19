using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Helpers;
using MyApplication.DataAccess.Contexts;
using MyApplication.DataAccess.Repositories;
using MyApplication.DataAccess.Setup;

namespace MyApplication.DataAccess;

public static class Startup
{
    public static IServiceCollection RegisterDataAccess(this IServiceCollection services)
    {
        var server = EnvironmentHelper.Get("APP_DB_SERVER")!;
        var database = EnvironmentHelper.Get("APP_DB_NAME")!;
        var username = EnvironmentHelper.Get("APP_DB_USER")!;
        var password = EnvironmentHelper.Get("APP_DB_PASS")!;
        var port = EnvironmentHelper.Get("APP_DB_PORT")!;
        var connectionString =
            $"Server={server}; Port={port};Database={database};User Id={username}; Password={password};";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, builder =>
            {
                builder.CommandTimeout(120);
                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });
        services.AddDbContext<AccountDbContext>(options =>
        {
            options.UseNpgsql(connectionString, builder =>
            {
                builder.CommandTimeout(120);
                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<IDatabaseMigrator, DatabaseMigrator>();
        return services;
    }
}