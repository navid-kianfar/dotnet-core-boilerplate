namespace MyApplication.Abstraction.Tests;

public interface IAccountControllerIntegrationTests
{
    Task Register_ShouldSucceed_UserDoesNotExists();
    Task Register_ShouldFail_UserDoesExists();
    Task Login_ShouldSucceed_CredentialsAreValid();
    Task Login_ShouldFail_CredentialsAreInvalid();
    Task GetProfile_ShouldSucceed_TokenIsValid();
}