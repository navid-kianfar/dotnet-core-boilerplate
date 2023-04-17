using MyApplication.DataAccess.Tables;

namespace MyApplication.Tests.Integration.Setup;

public class DatabaseSeed
{
    public readonly User[] Users;
    
    public DatabaseSeed()
    {
        var now = DateTime.UtcNow;
        Users = new[]
        {
            new User
            {
                
            }
        };
    }
}