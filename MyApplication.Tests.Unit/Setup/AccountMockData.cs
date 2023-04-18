using MyApplication.Abstraction.Dtos.Account;
using MyApplication.Abstraction.Types;
using MyApplication.Business.Helpers;

namespace MyApplication.Tests.Unit.Setup;

public static class AccountMockData
{
    public const string Password = "1234567890";
    public const string Salt = "MQlcQypPh+/KkoK4/Zstnzrb/godoOVs4loMMrWezSs=";
    
    public static UserDto[] Users = {
        new UserDto
        {
            Id = IncrementalGuid.NewId(),
            Username = "username1@myapplication.com",
            Hash = CryptographyHelper.Hash(Password, Salt),
            CreatedAt = DateTime.UtcNow,
            Salt = Salt
        },
        new UserDto
        {
            Id = IncrementalGuid.NewId(),
            Username = "username2@myapplication.com",
            Hash = CryptographyHelper.Hash(Password, Salt),
            CreatedAt = DateTime.UtcNow,
            Salt = Salt
        },
        new UserDto
        {
            Id = IncrementalGuid.NewId(),
            Username = "username3@myapplication.com",
            Hash = CryptographyHelper.Hash(Password, Salt),
            CreatedAt = DateTime.UtcNow,
            Salt = Salt
        },
    };

}