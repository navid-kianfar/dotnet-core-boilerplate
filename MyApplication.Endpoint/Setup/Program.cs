using MyApplication.Abstraction.Helpers;

namespace MyApplication.Endpoint;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.Title = "MyApplication Endpoint";
        EnvironmentHelper.Configure();
        var appIp = EnvironmentHelper.Get("APP_IP")!;
        var appPort = EnvironmentHelper.Get("APP_PORT")!;
        var builder = WebApplication.CreateBuilder(args);
        if (!EnvironmentHelper.IsTest())
            builder.WebHost.UseUrls($"http://{appIp}:{appPort}");
        builder.Services.RegisterApp();
        var app = builder.Build();
        app.UseAppServices();
        if (EnvironmentHelper.IsTest()) await app.RunAsync();
        else await app.RunAsync();
    }
}