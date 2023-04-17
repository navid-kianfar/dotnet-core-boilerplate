using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyApplication.Abstraction.Helpers;
using MyApplication.Abstraction.Types;

namespace MyApplication.Tests.Integration.Setup;

internal abstract class ApiIntegrationTestBase<S, T, C> where S : class
{
    private readonly string _apiPrefix;

    protected readonly HttpClient _client;

    protected readonly WebApplicationFactory<S> _factory;

    protected T _databaseSeed;

    protected ApiIntegrationTestBase(string apiPrefix)
    {
        EnvironmentHelper.Configure();
        _apiPrefix = apiPrefix;
        _factory = new WebApplicationFactory<S>();
        Action<IWebHostBuilder> builder = ApiIntegrationTestBase<S, T, C>.u003cu003ec.u003cu003e9__4_0;
        if (builder == null)
        {
            builder = new Action<IWebHostBuilder>(ApiIntegrationTestBase<S, T, C>.u003cu003ec.u003cu003e9,
                (IWebHostBuilder builder) =>
                {
                    var webHostBuilder = builder;
                    Action<IServiceCollection> u003cu003e9_41 =
                        ApiIntegrationTestBase<S, T, C>.u003cu003ec.u003cu003e9__4_1;
                    if (u003cu003e9_41 == null)
                    {
                        u003cu003e9_41 = new Action<IServiceCollection>(
                            ApiIntegrationTestBase<S, T, C>.u003cu003ec.u003cu003e9, (IServiceCollection services) => { });
                        ApiIntegrationTestBase<S, T, C>.u003cu003ec.u003cu003e9__4_1 = u003cu003e9_41;
                    }

                    webHostBuilder.ConfigureServices(u003cu003e9_41);
                });
            ApiIntegrationTestBase<S, T, C>.u003cu003ec.u003cu003e9__4_0 = builder;
        }

        _factory.WithWebHostBuilder(builder);
        _client = _factory.CreateClient();
    }

    protected async Task AuthenticateClient(string? token = null)
    {
        var authenticationToken = string.IsNullOrWhiteSpace(token) ? await GetAuthenticationToken() : token;
        Console.WriteLine("Token: {0}", authenticationToken);
    }

    private Task<string> GetAuthenticationToken()
    {
        return Task.FromResult<string>("");
    }

    protected async Task<OperationResult<X>> GetJsonAsync<X>(string route)
    {
        var str = string.Concat(_apiPrefix, "/", route);
        var async = await _client.GetAsync(str);
        var httpResponseMessage = async;
        async = null;
        httpResponseMessage.EnsureSuccessStatusCode();
        HttpContent content = httpResponseMessage.get_Content();
        var cancellationToken = new CancellationToken();
        OperationResult<X> operationResult =
            await HttpContentJsonExtensions.ReadFromJsonAsync<OperationResult<X>>(content, null, cancellationToken);
        var operationResult1 = operationResult;
        operationResult = null;
        var operationResult2 = operationResult1;
        if (operationResult2 == null) operationResult2 = OperationResult<X>.Fail("OPERATION_FAILED", default(X));

        var operationResult3 = operationResult2;
        str = null;
        httpResponseMessage = null;
        operationResult1 = null;
        return operationResult3;
    }

    protected virtual Task OnSeedingDatabase(C dbContext, T seedData)
    {
        return Task.CompletedTask;
    }

    protected async Task<OperationResult<X>> PostJsonAsync<X>(string route, object payload)
    {
        var str = string.Concat(_apiPrefix, "/", route);
        var httpClient = _client;
        var str1 = str;
        var obj = payload;
        var cancellationToken = new CancellationToken();
        var httpResponseMessage =
            await HttpClientJsonExtensions.PostAsJsonAsync<object>(httpClient, str1, obj, null, cancellationToken);
        var httpResponseMessage1 = httpResponseMessage;
        httpResponseMessage = null;
        httpResponseMessage1.EnsureSuccessStatusCode();
        HttpContent content = httpResponseMessage1.get_Content();
        cancellationToken = new CancellationToken();
        OperationResult<X> operationResult =
            await HttpContentJsonExtensions.ReadFromJsonAsync<OperationResult<X>>(content, null, cancellationToken);
        var operationResult1 = operationResult;
        operationResult = null;
        var operationResult2 = operationResult1;
        if (operationResult2 == null) operationResult2 = OperationResult<X>.Fail("OPERATION_FAILED", default(X));

        var operationResult3 = operationResult2;
        str = null;
        httpResponseMessage1 = null;
        operationResult1 = null;
        return operationResult3;
    }

    [SetUp]
    protected virtual async Task PrepareMockData()
    {
        ApiIntegrationTestBase<S, T, C>.u003cPrepareMockDatau003ed__9 variable = null;
        var asyncTaskMethodBuilder = AsyncTaskMethodBuilder.Create();
        asyncTaskMethodBuilder.Start<ApiIntegrationTestBase<S, T, C>.u003cPrepareMockDatau003ed__9>(ref variable);
        return asyncTaskMethodBuilder.get_Task();
    }
}