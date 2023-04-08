using Microsoft.EntityFrameworkCore;
using MyApplication.DataAccess.Tables;

namespace MyApplication.DataAccess.Contexts;

internal class AccountDbContext : DbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}