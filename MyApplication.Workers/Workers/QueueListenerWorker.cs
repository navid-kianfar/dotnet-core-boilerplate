using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Enums;
using MyApplication.Abstraction.Types;

namespace MyApplication.Workers.Workers;

/// <summary>
///     This is a base queue listener that will listen on a queue and will trigger when an item is pushed to it
///     and ExecuteAction method will be called with the input type casted to the given input type
///     if the process is successful the item will automatically acked from the queue
///     if process fails item will be rejected and goes back to the queue
///     Note: in real scenarios here we have to add a fail retry mechanism or something but it depends
///     on the application business (you may need to process items in pushed order so you can not go to the next node)
/// </summary>
/// <typeparam name="T">Input type</typeparam>
/// <typeparam name="Y">Output type</typeparam>
internal abstract class QueueListenerWorker<T, Y> : BackgroundService where T : class
{
    private readonly string _actionName;
    private readonly IJsonService _jsonService;
    private readonly ILoggerService _logger;
    private readonly string _queueName;
    private readonly IQueueService _queueService;

    protected QueueListenerWorker(
        string actionName,
        IJsonService jsonService,
        IQueueService queueService,
        ILoggerService logger)
    {
        _actionName = actionName;
        _jsonService = jsonService;
        _queueService = queueService;
        _logger = logger;
        _queueName = queueService.GetQueueName(actionName);
    }

    private string GetSection(string methodName)
    {
        return $"Workers.{_actionName}.{methodName}";
    }

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queueService.Subscribe(_queueName, async content =>
        {
            try
            {
                var item = _jsonService.Deserialize<T>(content);
                if (item == null) throw new ApplicationException("OBJECT_IN_QUEUE_IS_NULL");

                var response = await ExecuteAction(item, stoppingToken);
                return response.Status == OperationResultStatus.Success;
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, GetSection("ExecuteSubscriptionAsync"), e);
                return false;
            }
        });
    }

    protected virtual Task<OperationResult<Y>> ExecuteAction(T item, CancellationToken stoppingToken)
    {
        return Task.FromResult(OperationResult<Y>.Success());
    }
}