using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyApplication.Abstraction.Helpers;
using MyApplication.Abstraction.Types;
using NUnit.Framework;

namespace MyApplication.Tests.Integration.Setup;

internal abstract class EndpointIntegrationTestBase<S, T, C> where S : class
{
    private readonly string _apiPrefix;
    protected readonly HttpClient _client;
    protected readonly WebApplicationFactory<S> _factory;
    protected T _databaseSeed;

    protected EndpointIntegrationTestBase(string apiPrefix)
    {
        EnvironmentHelper.Configure();
        _apiPrefix = apiPrefix;
        _factory = new WebApplicationFactory<S>();
        _factory.WithWebHostBuilder(builder => { builder.ConfigureServices(services => { }); });
        _client = _factory.CreateClient();
    }

    protected async Task<OperationResult<X>> GetJsonAsync<X>(string route)
    {
        var endpoint = $"{_apiPrefix}/{route}";
        var content = await _client.GetAsync(endpoint);
        content.EnsureSuccessStatusCode();
        var result = await content.Content.ReadFromJsonAsync<OperationResult<X>>();
        return result ?? OperationResult<X>.Failed();
    }

    protected async Task<OperationResult<X>> PostJsonAsync<X>(string route, object payload)
    {
        var endpoint = $"{_apiPrefix}/{route}";
        var content = await _client.PostAsJsonAsync(endpoint, payload);
        content.EnsureSuccessStatusCode();
        var result = await content.Content.ReadFromJsonAsync<OperationResult<X>>();
        return result ?? OperationResult<X>.Failed();
    }

    protected async Task AuthenticateClient(string? token = null)
    {
        token ??= await OnGeneratingToken(_databaseSeed);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
    }

    [SetUp]
    protected virtual async Task PrepareMockData()
    {
        _databaseSeed = Activator.CreateInstance<T>();
        using (var scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var dbRepository = scope.ServiceProvider.GetService<C>()!;
            try
            {
                await OnSeedingDatabase(dbRepository, _databaseSeed);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                if (dbRepository is IDisposable disposable) disposable.Dispose();
            }
        }
    }

    protected virtual Task OnSeedingDatabase(C dbContext, T seedData)
    {
        return Task.CompletedTask;
    }

    protected virtual Task<string> OnGeneratingToken(T seedData)
    {
        return Task.FromResult(string.Empty);
    }
}