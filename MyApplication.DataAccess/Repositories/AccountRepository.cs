using Microsoft.EntityFrameworkCore;
using MyApplication.Abstraction.Contracts;
using MyApplication.Abstraction.Dtos.Account;
using MyApplication.DataAccess.Contexts;
using MyApplication.DataAccess.Tables;

namespace MyApplication.DataAccess.Repositories;

internal class AccountRepository : IAccountRepository, IDisposable
{
    private readonly AccountDbContext _dbContext;
    private readonly ILoggerService _loggerService;

    public AccountRepository(AccountDbContext dbContext, ILoggerService loggerService)
    {
        _dbContext = dbContext;
        _loggerService = loggerService;
    }

    public async Task<UserDto?> GetUser(Guid id)
    {
        try
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
            return user?.ToDto();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountRepository.GetUser", e);
            return null;
        }
    }

    public async Task<UserDto?> GetUser(string username)
    {
        try
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Username == username);
            return user?.ToDto();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountRepository.GetUser", e);
            return null;
        }
    }

    public async Task<bool> CreateUser(UserDto user)
    {
        try
        {
            await _dbContext.Users.AddAsync(User.FromDto(user));
            var affected = await _dbContext.SaveChangesAsync();
            return affected > 0;
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountRepository.CreateUser", e);
            return false;
        }
    }

    public async Task<bool> FailedLoginAttempt(Guid userId, bool block)
    {
        try
        {
            // Using Z.EntityFramework.Extensions.EFCore to update the record without loading it into memory
            DateTime? blockedAt = block ? DateTime.UtcNow : null;
            var affected = await _dbContext.Users
                .Where(i => i.Id == userId)
                .UpdateFromQueryAsync(i => new User
                {
                    BlockedAt = blockedAt,
                    FailedAttempt = i.FailedAttempt + 1
                });
            return affected > 0;
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "AccountRepository.FailedLoginAttempt", e);
            return false;
        }
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}