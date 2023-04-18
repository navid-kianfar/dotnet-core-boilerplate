using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Dtos.Storage;
using MyApplication.Abstraction.Types;

namespace MyApplication.Business.Core;

internal class StorageService : IStorageService
{
    public Task<OperationResult<StorageItemDto>> Upload(StorageItemDto file, string bucketName, string path)
        => Upload(file.Stream!, file.FileName, bucketName, path);
    
    public Task<OperationResult<StorageItemDto>> Upload(Stream stream, string fileName, string bucketName, string path)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<StorageItemDto>> Download(string path, string bucketName)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Delete(string path, string bucketName)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> Exists(string path, string bucketName)
    {
        throw new NotImplementedException();
    }
}