using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Helpers;

namespace MyApplication.Workers;

public class Program
{
    public static async Task Main(params string[] args)
    {
        Console.Title = "MyApplication Workers";
        EnvironmentHelper.Configure();

        var app = Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(services => services.RegisterApp())
            .Build();

        if (!EnvironmentHelper.IsDevelopment())
        {
            // Migrate the database to latest version & seed
            await using var scope = app.Services.CreateAsyncScope();
            var migrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>();
            await migrator.MigrateToLatestVersion();
            await migrator.Seed();
        }

        await app.RunAsync();
    }
}