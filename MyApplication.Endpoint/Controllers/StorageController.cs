using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Dtos.Storage;
using MyApplication.Abstraction.Fixtures;
using MyApplication.Abstraction.Types;
using MyApplication.Endpoint.Extensions;

namespace MyApplication.Endpoint.Controllers;

[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Storage")]
public class StorageController : BaseController
{
    private readonly IStorageManager _storageManager;

    public StorageController(IStorageManager storageManager)
    {
        _storageManager = storageManager;
    }
    
    [HttpPost]
    [AllowAnonymous]
    [Route(EndpointConstants.UploadPublic)]
    public async Task<OperationResult<StorageItemDto>> UploadPublic()
    {
        if (!Request.Form.Files.Any()) 
            return OperationResult<StorageItemDto>.Rejected();

        var item = await Request.Form.Files.First().ToStorageItem();
        return await _storageManager.UploadPublic(item);
    }
    
    [HttpPost]
    [Route(EndpointConstants.UploadProtected)]
    public async Task<OperationResult<StorageItemDto>> UploadProtected()
    {
        if (!Request.Form.Files.Any()) 
            return OperationResult<StorageItemDto>.Rejected();

        var item = await Request.Form.Files.First().ToStorageItem();
        return await _storageManager.UploadProtected(UserAuth!.UserId, item);
    }
    
    [HttpGet]
    [AllowAnonymous]
    [Route(EndpointConstants.DownloadPublic)]
    public async Task<IActionResult> DownloadPublic(string path)
    {
        var item = await _storageManager.DownloadPublic(path);
        if (item == null) return NotFound();

        return File(item.Stream!, item.MimeType, item.FileName);
    }
    
    [HttpPost]
    [Route(EndpointConstants.DownloadProtected)]
    public async Task<IActionResult> DownloadProtected(string path)
    {
        var item = await _storageManager.DownloadProtected(UserAuth!.UserId, path);
        if (item == null) return NotFound();

        return File(item.Stream!, item.MimeType, item.FileName);
    }
}