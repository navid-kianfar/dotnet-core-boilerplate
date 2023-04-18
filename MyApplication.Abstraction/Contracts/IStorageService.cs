using MyApplication.Abstraction.Dtos.Storage;
using MyApplication.Abstraction.Types;

namespace MyApplication.Abstraction.Contracts;

public interface IStorageService
{
    Task<OperationResult<StorageItemDto>> Upload(
        Stream stream, 
        string fileName, 
        string bucketName, 
        string path
    );
    
    Task<OperationResult<StorageItemDto>> Upload(
        StorageItemDto file, 
        string bucketName,
        string path
    );
    
    Task<OperationResult<StorageItemDto>> Download(string path, string bucketName);
    
    Task<OperationResult<bool>> Delete(string path, string bucketName);
    Task<OperationResult<bool>> Exists(string path, string bucketName);
}