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
        await app.RunAsync();
    }
}