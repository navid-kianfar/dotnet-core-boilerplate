using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Dtos.Account;
using MyApplication.Abstraction.Enums;
using MyApplication.Abstraction.Fixtures;
using MyApplication.Abstraction.Types;
using MyApplication.Business.Helpers;

namespace MyApplication.Business.Account;

internal class AccountService : IAccountService
{
    private const int MaxFailedAttempt = 5;
    private readonly ILoggerService _loggerService;
    private readonly IQueueService _queueService;
    private readonly IAccountRepository _repository;

    public AccountService(
        IAccountRepository repository,
        ILoggerService loggerService,
        IQueueService queueService
    )
    {
        _repository = repository;
        _loggerService = loggerService;
        _queueService = queueService;
    }

    #region Endpoint

    public async Task<OperationResult<UserDto?>> Login(LoginRequest request)
    {
        try
        {
            var user = await _repository.GetUser(request.Username);
            if (user == null)
                return OperationResult<UserDto?>.NotFound();

            if (user.BlockedAt.HasValue)
                return OperationResult<UserDto?>.Rejected(ErrorMessages.AccountIsBlocked);

            var verified = CryptographyHelper.Verify(request.Password, user.Hash, user.Salt);
            if (!verified)
            {
                user.FailedAttempt++;
                var block = user.FailedAttempt == MaxFailedAttempt;
                await _repository.FailedLoginAttempt(user.Id, block);
                if (block)
                    // Send to worker to do the long running process which has re-try mechanism built-in
                    await SendToWorker(ApplicationConstants.AccountQueue, new AccountBackgroundRequest
                    {
                        Id = user.Id,
                        Type = AccountActionType.Blocked
                    });
                return OperationResult<UserDto?>.Rejected(ErrorMessages.InvalidUsernameOrPassword);
            }

            return OperationResult<UserDto?>.Success(user);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountService.Login", e);
            return OperationResult<UserDto?>.Failed();
        }
    }

    public async Task<OperationResult<UserDto?>> Register(RegisterRequest request)
    {
        try
        {
            var user = await _repository.GetUser(request.Username);
            if (user != null)
                return OperationResult<UserDto?>.Duplicate();

            var salt = CryptographyHelper.GenerateSalt();
            var hash = CryptographyHelper.Hash(request.Password, salt);
            user = new UserDto
            {
                Id = IncrementalGuid.NewId(),
                Username = request.Username,
                CreatedAt = DateTime.UtcNow,
                Hash = hash,
                Salt = salt
            };

            var saved = await _repository.CreateUser(user);
            if (!saved) return OperationResult<UserDto?>.Failed();

            // Send to worker to do the long running process which has re-try mechanism built-in
            await SendToWorker(ApplicationConstants.AccountQueue, new AccountBackgroundRequest
            {
                Id = user.Id,
                Type = AccountActionType.Created
            });

            return OperationResult<UserDto?>.Success(user);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountService.Register", e);
            return OperationResult<UserDto?>.Failed();
        }
    }

    public async Task<OperationResult<UserDto?>> GetUser(Guid userId)
    {
        try
        {
            var user = await _repository.GetUser(userId);
            if (user == null)
                return OperationResult<UserDto?>.NotFound();

            return OperationResult<UserDto?>.Success(user);
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountService.Profile", e);
            return OperationResult<UserDto?>.Failed();
        }
    }


    private async Task SendToWorker(string actionName, object data)
    {
        var queueName = _queueService.GetQueueName(actionName);
        await _queueService.Enqueue(queueName, data);
    }

    #endregion

    #region Background Workers

    /// <summary>
    ///     This method will be called when background worker gets an item to process
    /// </summary>
    /// <param name="item">The account the has been affected by the operation</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException">If the operation is UnKnown</exception>
    public Task<OperationResult<bool>> ProcessAccountBackgroundRequest(AccountBackgroundRequest item,
        CancellationToken cancellationToken)
    {
        switch (item.Type)
        {
            case AccountActionType.Created:
                return OnAccountCreated(item.Id, cancellationToken);
            case AccountActionType.Blocked:
                return OnAccountBlocked(item.Id, cancellationToken);
            case AccountActionType.Deleted:
                return OnAccountDeleted(item.Id, cancellationToken);
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     When an account is deleted this long running process will be executed
    /// </summary>
    /// <param name="accountId">The id of the account that has been affected by the operation</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<OperationResult<bool>> OnAccountDeleted(Guid accountId, CancellationToken cancellationToken)
    {
        await _loggerService.Info($"Account {accountId} has been deleted", "AccountService.OnAccountDeleted");
        // TODO: there may be a need to send an email or delete user files according to the logic of the application
        return OperationResult<bool>.Success(true);
    }

    /// <summary>
    ///     When an account is blocked this long running process will be executed
    /// </summary>
    /// <param name="accountId">The id of the account that has been affected by the operation</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<OperationResult<bool>> OnAccountBlocked(Guid accountId, CancellationToken cancellationToken)
    {
        await _loggerService.Info($"Account {accountId} has been blocked", "AccountService.OnAccountBlocked");
        // TODO: there may be a need to send an email or delete user files according to the logic of the application
        return OperationResult<bool>.Success(true);
    }

    /// <summary>
    ///     When an account is created this long running process will be executed
    /// </summary>
    /// <param name="accountId">The id of the account that has been affected by the operation</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<OperationResult<bool>> OnAccountCreated(Guid accountId, CancellationToken cancellationToken)
    {
        await _loggerService.Info($"Account {accountId} has been created", "AccountService.OnAccountCreated");
        // TODO: there may be a need to send a verification email according to the logic of the application
        return OperationResult<bool>.Success(true);
    }

    #endregion
}