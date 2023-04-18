using MyApplication.Abstraction.Dtos.Account;
using MyApplication.Abstraction.Enums;
using MyApplication.Abstraction.Fixtures;
using MyApplication.Abstraction.Tests;
using MyApplication.DataAccess.Contexts;
using MyApplication.Endpoint.Services;
using MyApplication.Endpoint.Setup;
using MyApplication.Tests.Integration.Setup;
using NUnit.Framework;

namespace MyApplication.Tests.Integration.Controllers;

[TestFixture]
internal class AccountControllerTests: 
    EndpointIntegrationTestBase<TestStartup, DatabaseSeed, ApplicationDbContext>,
    IAccountControllerIntegrationTests
{
    public AccountControllerTests() : base(EndpointConstants.Prefix)
    {
        
    }

    protected override Task<string> OnGeneratingToken(DatabaseSeed seedData)
    {
        var user = seedData.Users.First().ToDto();
        var token = TokenService.GenerateToken(user);
        return Task.FromResult(token);
    }

    protected override async Task OnSeedingDatabase(ApplicationDbContext dbContext, DatabaseSeed seedData)
    {
        await dbContext.Users.DeleteFromQueryAsync();
        await dbContext.Users.AddRangeAsync(seedData.Users);
        await dbContext.SaveChangesAsync();
    }

    [Test]
    public async Task Login_ShouldSucceed_CredentialsAreValid()
    {
        
        var request = new LoginRequest()
        {
            Username = _databaseSeed.Users.First().Username,
            Password = _databaseSeed.Password
        };

        var test = await PostJsonAsync<LoginResponse>(EndpointConstants.Login, request);

        //assert
        Assert.NotNull(test);

        Assert.That(test!.Status, Is.EqualTo(OperationResultStatus.Success));

        Assert.IsNotNull(test.Data);

        Assert.IsNotEmpty(test.Message, test.Data!.Token);
    }

    [Test]
    public async Task Login_ShouldFail_CredentialsAreInvalid()
    {
        var request = new LoginRequest()
        {
            Username = _databaseSeed.Users.First().Username,
            Password = Guid.NewGuid().ToString()
        };

        var test = await PostJsonAsync<LoginResponse>(EndpointConstants.Login, request);

        //assert
        Assert.NotNull(test);

        Assert.That(test!.Status, Is.EqualTo(OperationResultStatus.Failed));

        Assert.IsNull(test.Data);

        Assert.That(ErrorMessages.InvalidUsernameOrPassword, Is.EqualTo(test.Message));
    }

    [Test]
    public async Task GetProfile_ShouldSucceed_TokenIsValid()
    {
        await AuthenticateClient();
        var test = await GetJsonAsync<ProfileResponse>(EndpointConstants.Login);

        //assert
        Assert.NotNull(test);

        Assert.That(test!.Status, Is.EqualTo(OperationResultStatus.Success));

        Assert.NotNull(test.Data);
    }

    [Test]
    public async Task Register_ShouldSucceed_UserDoesNotExists()
    {
        var request = new RegisterRequest()
        {
            Username = $"{Guid.NewGuid()}@my-application.com",
            Password = _databaseSeed.Password
        };

        var test = await PostJsonAsync<RegisterResponse>(EndpointConstants.Register, request);

        //assert
        Assert.NotNull(test);

        Assert.That(test!.Status, Is.EqualTo(OperationResultStatus.Success));

        Assert.NotNull(test.Data);

        Assert.IsNotEmpty(test.Data!.Token);
    }

    [Test]
    public async Task Register_ShouldFail_UserDoesExists()
    {
        var request = new RegisterRequest()
        {
            Username = _databaseSeed.Users.First().Username,
            Password = _databaseSeed.Password
        };

        var test = await PostJsonAsync<RegisterResponse>(EndpointConstants.Register, request);

        //assert
        Assert.NotNull(test);

        Assert.That(test!.Status, Is.EqualTo(OperationResultStatus.Failed));

        Assert.NotNull(test.Data);

        Assert.That(ErrorMessages.UserAlreadyExists, Is.EqualTo(test.Message));
    }
}