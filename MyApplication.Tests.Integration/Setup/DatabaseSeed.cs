using MyApplication.DataAccess.Tables;

namespace MyApplication.Tests.Integration.Setup;

internal class DatabaseSeed
{
    public readonly User[] Users;

    public string Password { get; set; }
    
    public DatabaseSeed()
    {
        Password = Guid.NewGuid().ToString();
        var now = DateTime.UtcNow;
        Users = new[]
        {
            new User
            {
                
            }
        };
    }
}