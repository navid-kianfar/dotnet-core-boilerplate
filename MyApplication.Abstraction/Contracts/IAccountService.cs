using MyApplication.Abstraction.Dtos.Account;
using MyApplication.Abstraction.Types;

namespace MyApplication.Abstraction.Contracts;

public interface IAccountService
{
    Task<OperationResult<UserDto?>> Login(LoginRequest request);
    Task<OperationResult<UserDto?>> Register(RegisterRequest request);
    Task<OperationResult<UserDto?>> GetUser(Guid userId);
    Task<OperationResult<bool>> ProcessAccountBackgroundRequest(AccountBackgroundRequest item, CancellationToken token);
}