using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyApplication.Abstraction.Helpers;
using MyApplication.Abstraction.Types;
using NUnit.Framework;

namespace MyApplication.Tests.Integration.Setup;

internal abstract class ApiIntegrationTestBase<S, T, C> where S : class
{
    private readonly string _apiPrefix;
    protected T _databaseSeed;
    protected readonly HttpClient _client;
    protected readonly WebApplicationFactory<S> _factory;

    protected ApiIntegrationTestBase(string apiPrefix)
    {
        _apiPrefix = apiPrefix;
        EnvironmentHelper.Configure();
        _factory = new WebApplicationFactory<S>();
        _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                
            });
        });
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
        token ??= await GetAuthenticationToken();
        Console.WriteLine("Token: {0}", token);
        // _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
    }

    private Task<string> GetAuthenticationToken()
    {
        return Task.FromResult("");
    }
    
    [SetUp]
    protected virtual async Task PrepareMockData()
    {
        _databaseSeed = Activator.CreateInstance<T>();
        using (var scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var dbRepository = (scope.ServiceProvider.GetService<C>())!;
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
                if (dbRepository is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }

    protected virtual Task OnSeedingDatabase(C dbContext, T seedData) => Task.CompletedTask;
}