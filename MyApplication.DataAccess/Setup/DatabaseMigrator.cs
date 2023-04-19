using Microsoft.EntityFrameworkCore;
using MyApplication.Abstraction.Contracts;
using MyApplication.DataAccess.Contexts;

namespace MyApplication.DataAccess.Setup;

internal class DatabaseMigrator : IDatabaseMigrator
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseMigrator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task MigrateToLatestVersion()
    {
        await _dbContext.Database.EnsureCreatedAsync();
        await _dbContext.Database.MigrateAsync();
    }

    public Task Seed()
    {
        // TODO: you may seed the database here...
        return Task.CompletedTask;
    }
}