# MyApplication - Boilerplate

## MyApplication.Abstraction

## MyApplication.Business

## MyApplication.DataAccess

## MyApplication.Endpoint
Open api end point for the application

### BaseController
The base controller for all the apis that forces authorization and handles the AuthUser data in the pipeline

### AccountController
implements the basic operations for account management
* `Task<OperationResult<ProfileResponse>> Profile()`
* `Task<OperationResult<LoginResponse>> Login(LoginRequest request)`
* `Task<OperationResult<RegisterResponse>> Register(RegisterRequest request)`

### StorageController
Implements the basic operation for uploading/downloading the objects from minio object storage
* `Task<OperationResult<StorageItemDto>> UploadPublic()`
* `Task<OperationResult<StorageItemDto>> UploadProtected()`
* `Task<IActionResult> DownloadPublic(string path)`
* `Task<IActionResult> DownloadProtected(string path)`

## MyApplication.Workers

### QueueListenerWorker
This is a base worker that will listen on a queue and will trigger when an item is pushed to it
and **ExecuteAction** method will be called with the input type casted to the given input type
if the process is successful the item will automatically acked from the queue
if process fails item will be rejected and goes back to the queue
Note: in real scenarios here we have to add a fail retry mechanism or something but it depends
on the application business (you may need to process items in pushed order so you can not go to the next item)
### AccountBackgroundWorker
This background worker is responsible to long running processes related to actions
affecting an account, there might be a need to send an verification email or an otp...

## MyApplication.Tests.Integration
The integration tests of each controller will be in a separate file 

### DatabaseSeed
This is a class holding variables that are used in the tests and also the **OnSeedingDatabase** will be executed before each test

### EndpointIntegrationTestBase
EndpointIntegrationTestBase is a generic class that accepts a Startup, DatabaseSeed and a db context.
it accepts a base url prefix and exposes the following methods:

* `Task<OperationResult<X>> GetJsonAsync<X>(string route)`
* `Task<OperationResult<X>> PostJsonAsync<X>(string route, object payload)`
* `Task AuthenticateClient(string? token = null)` - this method will be used to set the token for the dispatching request
* `Task<string> OnGeneratingToken(T seedData)` - this method will be called to generate the token that will attach to the header of dispatching request
* `Task OnSeedingDatabase(C dbContext, T seedData)` - this method will run to seed the database, for example: 
``` csharp
protected override async Task OnSeedingDatabase(ApplicationDbContext dbContext, DatabaseSeed seedData)
{
    await dbContext.Users.DeleteFromQueryAsync();
    await dbContext.Users.AddRangeAsync(seedData.Users);
    await dbContext.SaveChangesAsync();
}
```
### AccountControllerTests
The integration tests for account controller

``` csharp
internal class AccountControllerTests :
    EndpointIntegrationTestBase<TestStartup, DatabaseSeed, ApplicationDbContext>,
    IAccountControllerIntegrationTests
{
    public AccountControllerTests() : base(EndpointConstants.Prefix)
    {
    }
}
```
it implements the **IAccountControllerIntegrationTests** interface
``` csharp
public interface IAccountControllerIntegrationTests
{
    Task Register_ShouldSucceed_UserDoesNotExists();
    Task Register_ShouldFail_UserDoesExists();
    Task Login_ShouldSucceed_CredentialsAreValid();
    Task Login_ShouldFail_CredentialsAreInvalid();
    Task GetProfile_ShouldSucceed_TokenIsValid();
}
```

## MyApplication.Tests.Unit

### AccountServiceTests
The unit tests for account service
it implements the **IAccountServiceTests** interface
``` csharp
public interface IAccountServiceTests
{
    Task Register_ShouldSucceed_UserDoesNotExists();
    Task Register_ShouldFail_UserDoesExists();
    Task Login_ShouldSucceed_CredentialsAreValid();
    Task Login_ShouldFail_CredentialsAreInvalid();
    Task GetProfile_ShouldSucceed_TokenIsValid();
}
```
 