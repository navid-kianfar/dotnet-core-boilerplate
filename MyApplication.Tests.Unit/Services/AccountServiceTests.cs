using Moq;
using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Dtos.Account;
using MyApplication.Abstraction.Enums;
using MyApplication.Abstraction.Helpers;
using MyApplication.Abstraction.Tests;
using MyApplication.Abstraction.Types;
using MyApplication.Business.Account;
using MyApplication.Tests.Unit.Setup;
using NUnit.Framework;

namespace MyApplication.Tests.Unit.Services;

[TestFixture]
internal class AccountServiceTests : IAccountServiceTests
{
    private readonly IAccountService _service;
    private readonly Mock<ILoggerService> _loggerService = new();
    private readonly Mock<IQueueService> _queueService = new();
    private readonly Mock<IAccountRepository> _accountRepository = new();

    public AccountServiceTests()
    {
        EnvironmentHelper.Configure();
        _service = new AccountService(
            _accountRepository.Object,
            _loggerService.Object,
            _queueService.Object
        );
    }

    [Test]
    public async Task Register_ShouldSucceed_UserDoesNotExists()
    {
        var user = AccountMockData.Users.First();
        var request = new RegisterRequest()
        {
            Username = user.Username,
            Password = AccountMockData.Password
        };

        _accountRepository
            .Setup(i => i.GetUser(request.Username))
            .ReturnsAsync(((UserDto?)null));

        _accountRepository
            .Setup(i => i.CreateUser(It.IsAny<UserDto>()))
            .ReturnsAsync(true);

        var test = await _service.Register(request);
        
        //assert
        Assert.NotNull(test);

        Assert.That(test!.Status, Is.EqualTo(OperationResultStatus.Success));

        Assert.NotNull(test.Data);

        Assert.That(request.Username, Is.EqualTo(test.Data!.Username));
    }

    [Test]
    public async Task Register_ShouldFail_UserDoesExists()
    {
        var user = AccountMockData.Users.First();
        var request = new RegisterRequest()
        {
            Username = user.Username,
            Password = AccountMockData.Password
        };

        _accountRepository
            .Setup(i => i.GetUser(request.Username))
            .ReturnsAsync(user);

        //act
        var test = await _service.Register(request);

        //assert
        Assert.NotNull(test);

        Assert.That(test!.Status, Is.EqualTo(OperationResultStatus.Duplicate));

        Assert.Null(test.Data);
    }

    [Test]
    public async Task Login_ShouldSucceed_CredentialsAreValid()
    {
        var user = AccountMockData.Users.First();

        var request = new LoginRequest
        {
            Username = user.Username,
            Password = AccountMockData.Password
        };
        
        _accountRepository
            .Setup(i => i.GetUser(request.Username))
            .ReturnsAsync(user);

        //act
        var test = await _service.Login(request);

        //assert
        Assert.NotNull(test);

        Assert.That(test!.Status, Is.EqualTo(OperationResultStatus.Success));

        Assert.NotNull(test.Data);

        Assert.That(request.Username, Is.EqualTo(test.Data!.Username));
    }

    [Test]
    public async Task Login_ShouldFail_CredentialsAreInvalid()
    {
        var user = AccountMockData.Users.First();

        var request = new LoginRequest
        {
            Username = user.Username,
            Password = Guid.NewGuid().ToString()
        };

        _accountRepository
            .Setup(i => i.GetUser(request.Username))
            .ReturnsAsync(user);

        //act
        var test = await _service.Login(request);

        //assert
        Assert.NotNull(test);

        Assert.That(test!.Status, Is.EqualTo(OperationResultStatus.Rejected));

        Assert.Null(test.Data);
    }

    [Test]
    public async Task GetProfile_ShouldSucceed_TokenIsValid()
    {
        var user = AccountMockData.Users.First();

        _accountRepository
            .Setup(i => i.GetUser(user.Id))
            .ReturnsAsync(user);

        //act
        var test = await _service.GetUser(user.Id);

        //assert
        Assert.NotNull(test);

        Assert.That(test!.Status, Is.EqualTo(OperationResultStatus.Success));

        Assert.NotNull(test.Data);
    }
}