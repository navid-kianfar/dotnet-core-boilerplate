namespace MyApplication.Abstraction.Contracts;

public interface IDatabaseMigrator
{
    Task MigrateToLatestVersion();
    Task Seed();
}