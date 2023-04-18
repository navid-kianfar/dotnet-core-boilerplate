using MyApplication.Abstraction.Dtos.Storage;
using MyApplication.Abstraction.Types;

namespace MyApplication.Abstraction.Contracts;

public interface IStorageService
{
    public Task<OperationResult<StorageItemDto>> Upload(
        Stream stream, 
        string fileName, 
        string bucketName, 
        string path
    );
    
    public Task<OperationResult<StorageItemDto>> Upload(
        StorageItemDto file, 
        string bucketName, 
        string path
    );
    
    public Task<OperationResult<StorageItemDto>> DownloadPublic(string path);
    Task<OperationResult<StorageItemDto>> DownloadProtected(Guid userId, string path);
    
    public Task<OperationResult<bool>> Delete(string path, string bucketName);
    public Task<OperationResult<bool>> Exists(string path, string bucketName);
}