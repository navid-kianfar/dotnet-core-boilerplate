using MyApplication.Abstraction.Types;
using MyApplication.Business.Helpers;
using MyApplication.DataAccess.Tables;

namespace MyApplication.Tests.Integration.Setup;

internal class DatabaseSeed
{
    public const string Password = "1234567890";
    public const string Salt = "MQlcQypPh+/KkoK4/Zstnzrb/godoOVs4loMMrWezSs=";
    public readonly User[] Users;

    public DatabaseSeed()
    {
        var now = DateTime.UtcNow;
        Users = new[]
        {
            new User
            {
                Id = IncrementalGuid.NewId(),
                Username = "username1@myapplication.com",
                Hash = CryptographyHelper.Hash(Password, Salt),
                CreatedAt = DateTime.UtcNow,
                Salt = Salt
            }
        };
    }
}