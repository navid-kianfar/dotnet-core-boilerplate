using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyApplication.Abstraction.Helpers;

public static class EnvironmentHelper
{
    public static string? Get(string key)
    {
        return Environment.GetEnvironmentVariable(key);
    }

    public static void Set(string key, string value)
    {
        Environment.SetEnvironmentVariable(key, value);
    }

    public static string Env()
    {
        return Get("APP_ENV") ?? "";
    }

    public static bool IsDevelopment()
    {
        return Get("APP_ENV") == "Development";
    }

    public static bool IsProduction()
    {
        return Get("APP_ENV") == "Production";
    }

    public static bool IsTest()
    {
        return Get("APP_ENV") == "Test";
    }

    public static void Configure(string fileName = "launchSettings.json")
    {
        if (string.IsNullOrWhiteSpace(Env()))
        {
            var settingPath = GetAppSettingPath(fileName);
            if (File.Exists(settingPath))
            {
                var settingContent = File.ReadAllText(settingPath);
                var profiles = JsonConvert.DeserializeObject<JObject>(settingContent)!["profiles"]!;
                var profile = ((JObject)profiles).Properties().First().Name;
                var config = profiles[profile]!["environmentVariables"]!;
                foreach (var property in ((JObject)config).Properties())
                    Set(property.Name, config[property.Name]!.ToString());
            }
        }
    }

    private static string GetAppSettingPath(string fileName)
    {
        var cleaned = Environment.CurrentDirectory.Replace("\\", "/");
        return $"{cleaned.Split("/bin/")[0]}/Properties/{fileName}";
    }
}