using MyApplication.Abstraction.Dtos.Storage;
using MyApplication.Abstraction.Types;

namespace MyApplication.Abstraction.Contracts;

public interface IStorageManager
{
    Task<StorageItemDto?> DownloadPublic(string path);
    Task<StorageItemDto?> DownloadProtected(Guid userId, string path);
    Task<OperationResult<StorageItemDto>> UploadProtected(Guid userId, StorageItemDto item);
    Task<OperationResult<StorageItemDto>> UploadPublic(StorageItemDto item);
}