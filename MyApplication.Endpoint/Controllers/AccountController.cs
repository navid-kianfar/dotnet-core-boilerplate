using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Dtos.Account;
using MyApplication.Abstraction.Enums;
using MyApplication.Abstraction.Fixtures;
using MyApplication.Abstraction.Types;
using MyApplication.Endpoint.Services;

namespace MyApplication.Endpoint.Controllers;

[Route(EndpointConstants.Prefix)]
[ApiExplorerSettings(GroupName = "Account")]
public class AccountController : BaseController
{
    private readonly IAccountService _service;

    public AccountController(IAccountService service)
    {
        _service = service;
    }

    [HttpGet]
    [Route(EndpointConstants.Profile)]
    public async Task<OperationResult<ProfileResponse>> Profile()
    {
        var op = await _service.GetUser(UserAuth!.UserId);
        if (op.Status != OperationResultStatus.Success) return op.To<ProfileResponse>();
        if (op.Data!.BlockedAt.HasValue || op.Data!.DeletedAt.HasValue)
            return OperationResult<ProfileResponse>.UnAuthorized();
        return OperationResult<ProfileResponse>.Success(new ProfileResponse
        {
            Id = op.Data!.Id,
            Username = op.Data!.Username,
            CreatedAt = op.Data!.CreatedAt
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [Route(EndpointConstants.Login)]
    public async Task<OperationResult<LoginResponse>> Login(LoginRequest request)
    {
        var op = await _service.Login(request);
        if (op.Status != OperationResultStatus.Success) return op.To<LoginResponse>();
        var token = TokenService.GenerateToken(op.Data!);
        return OperationResult<LoginResponse>.Success(new LoginResponse { Token = token });
    }

    [HttpPost]
    [AllowAnonymous]
    [Route(EndpointConstants.Register)]
    public async Task<OperationResult<RegisterResponse>> Register(RegisterRequest request)
    {
        var op = await _service.Register(request);
        if (op.Status != OperationResultStatus.Success) return op.To<RegisterResponse>();
        var token = TokenService.GenerateToken(op.Data!);
        return OperationResult<RegisterResponse>.Success(new RegisterResponse { Token = token });
    }
}