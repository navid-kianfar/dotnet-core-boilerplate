using HeyRed.Mime;
using Minio;
using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Dtos.Storage;
using MyApplication.Abstraction.Helpers;
using MyApplication.Abstraction.Types;

namespace MyApplication.Business.Core;

internal class StorageService : IStorageService, IDisposable
{
    private readonly ILoggerService _loggerService;
    private readonly MinioClient _client;
    public StorageService(ILoggerService loggerService)
    {
        var endpoint = EnvironmentHelper.Get("APP_STORAGE_SERVER")!;
        var accessKey = EnvironmentHelper.Get("APP_STORAGE_USER")!;
        var secretKey = EnvironmentHelper.Get("APP_STORAGE_PASS")!;
        var port = int.Parse(EnvironmentHelper.Get("APP_STORAGE_PORT")!);
        _loggerService = loggerService;
        _client = new MinioClient()
            .WithEndpoint(endpoint, port)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false)
            .Build();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public Task<OperationResult<StorageItemDto>> Upload(StorageItemDto file, string bucketName, string path)
        => Upload(file.Stream!, file.FileName, bucketName, path);
    
    public async Task<OperationResult<StorageItemDto>> Upload(Stream stream, string fileName, string bucketName, string path)
    {
        try
        {
            await EnsureBucketExists(_client, bucketName, true);
            
            var destinationPath = path;
            if (!destinationPath.EndsWith(path)) 
                destinationPath = Path.Combine(destinationPath, fileName);
            
            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(destinationPath)
                .WithObjectSize(stream.Length)
                .WithStreamData(stream);
            
            await _client.PutObjectAsync(args);
            
            var ext = Path.GetExtension(fileName);
            
            return OperationResult<StorageItemDto>.Success(new StorageItemDto
            {
                Url = destinationPath,
                Extension = ext,
                FileName = fileName,
                FileSize = stream.Length,
                MimeType = MimeTypesMap.GetMimeType(ext),
                CreatedAt = DateTime.UtcNow
            });
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageService.Upload", e);
            return OperationResult<StorageItemDto>.Failed();
        }
    }

    public async Task<OperationResult<StorageItemDto>> Download(string path, string bucketName)
    {
        try
        {
            var stream = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(path)
                .WithCallbackStream(objectStream =>
                {
                    objectStream.CopyTo(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                });
            var response = await _client.GetObjectAsync(args);
            if (response == null) return OperationResult<StorageItemDto>.Failed();

            return OperationResult<StorageItemDto>.Success(new StorageItemDto
            {
                Url = path,
                Stream = stream,
                CreatedAt = DateTime.UtcNow,
                Extension = Path.GetExtension(path),
                FileName = Path.GetFileName(path),
                FileSize = response.Size,
                MimeType = response.ContentType,
            });
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageService.Download", e);
            return OperationResult<StorageItemDto>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Delete(string path, string bucketName)
    {
        try
        {
            var args = new RemoveObjectArgs().WithBucket(bucketName).WithObject(path);
            await _client.RemoveObjectAsync(args);
            return OperationResult<bool>.Success();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "StorageService.Delete", e);
            return OperationResult<bool>.Failed();
        }
    }

    public async Task<OperationResult<bool>> Exists(string path, string bucketName)
    {
        try
        {
            // Check whether the object exists using StatObjectAsync(). If the object is not found,
            // StatObjectAsync() will throw an exception.
            var args = new StatObjectArgs().WithBucket(bucketName).WithObject(path);
            await _client.StatObjectAsync(args);
            return OperationResult<bool>.Success();
        }
        catch (Exception e)
        {
            if (e.Message == "MinIO API responded with message=Not found.")
                return OperationResult<bool>.Success(false);
            await _loggerService.Error(e.Message, "StorageService.Exists", e);
            return OperationResult<bool>.Failed();
        }
    }
    
    private async Task<bool> EnsureBucketExists(MinioClient client, string bucketName, bool force)
    {
        var exists = await client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (exists || !force) return exists;
        await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        return true;
    }
}