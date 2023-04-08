using MyApplication.Abstraction.Dtos.Account;

namespace MyApplication.Abstraction.Contracts;

public interface IAccountRepository
{
    Task<UserDto?> GetUser(Guid id);
    Task<UserDto?> GetUser(string username);
    Task<bool> CreateUser(UserDto user);
    Task<bool> FailedLoginAttempt(Guid userId, bool block);
}