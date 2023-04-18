using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Dtos.Storage;
using MyApplication.Abstraction.Fixtures;
using MyApplication.Abstraction.Types;

namespace MyApplication.Business.Core;

internal class StorageManager : IStorageManager
{
    private readonly IStorageService _storageService;

    public StorageManager(IStorageService storageService)
    {
        _storageService = storageService;
    }
    
    public async Task<StorageItemDto?> DownloadPublic(string path)
    {
        if (CheckIfPathIsSafe(path)) return null;
        var op = await _storageService.Download(path, ApplicationConstants.PublicBucket);
        return op.Data;
    }

    public async Task<StorageItemDto?> DownloadProtected(Guid userId, string path)
    {
        if (CheckIfPathIsSafe(path, userId)) return null;
        var op = await _storageService.Download(path, ApplicationConstants.ProtectedBucket);
        return op.Data;
    }

    public Task<OperationResult<StorageItemDto>> UploadProtected(Guid userId, StorageItemDto item)
    {
        var path = $"{userId}/{GetPath(item.FileName)}";
        return _storageService.Upload(item, ApplicationConstants.ProtectedBucket, path);
    }

    public Task<OperationResult<StorageItemDto>> UploadPublic(StorageItemDto item)
    {
        var path = GetPath(item.FileName);
        return _storageService.Upload(item, ApplicationConstants.PublicBucket, path);
    }

    private string GetPath(string fileName) =>
        $"{DateTime.UtcNow:yyyy-MM-dd}/{IncrementalGuid.NewId()}/{fileName}"; 
    private bool CheckIfPathIsSafe(string path, Guid? userId = null) 
    {
        if (userId.HasValue)
            return !path.Contains(userId.Value.ToString());
        return !path.Contains("../");
    }
}