using MyApplication.Abstraction.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyApplication.Abstraction.Implementation;

internal class JsonService : IJsonService
{
    private readonly JsonSerializerSettings _settings;

    public JsonService()
    {
        _settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.Indented
        };
    }

    public T Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json)!;
    }

    public string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, _settings);
    }
}