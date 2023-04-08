using Microsoft.EntityFrameworkCore;
using MyApplication.DataAccess.Tables;

namespace MyApplication.DataAccess.Contexts;

internal class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}