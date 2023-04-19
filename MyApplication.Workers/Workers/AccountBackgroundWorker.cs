using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Dtos.Account;
using MyApplication.Abstraction.Fixtures;
using MyApplication.Abstraction.Types;

namespace MyApplication.Workers.Workers;

/// <summary>
///     This background worker is responsible to long running processes related to actions
///     affecting an account, there might be a need to send an verification email or an otp...
/// </summary>
internal class AccountBackgroundWorker : QueueListenerWorker<AccountBackgroundRequest, bool>
{
    private readonly IAccountService _accountService;
    private readonly ILoggerService _logger;

    public AccountBackgroundWorker(
        IAccountService accountService,
        IJsonService jsonService,
        IQueueService queueClient,
        ILoggerService logger) :
        base(ApplicationConstants.AccountQueue, jsonService, queueClient, logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    /// <summary>
    ///     There may be a need to load data or retrive some state before starting the worker
    /// </summary>
    /// <param name="cancellationToken"></param>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _logger.Log("StartAsync", "AccountBackgroundWorker");
        await base.StartAsync(cancellationToken);
    }

    /// <summary>
    ///     There may be a need to save some state before shutting down the worker
    /// </summary>
    /// <param name="cancellationToken"></param>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _logger.Log("StopAsync", "AccountBackgroundWorker");
        await base.StartAsync(cancellationToken);
    }

    /// <summary>
    ///     This method will be called when ever an item is being pushed to the account queue
    ///     There should be no logic in here and simply passing the data to AccountService to handle the logic
    /// </summary>
    /// <param name="item">Account that has been affected by the operation</param>
    /// <param name="stoppingToken"></param>
    /// <returns>
    ///     The operation result of the process, to process to the next item in queue the result must be a
    ///     OperationResult.Success(true)
    /// </returns>
    protected override Task<OperationResult<bool>> ExecuteAction(AccountBackgroundRequest item,
        CancellationToken stoppingToken)
    {
        return _accountService.ProcessAccountBackgroundRequest(item, stoppingToken);
    }
}