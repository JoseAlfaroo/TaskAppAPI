using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Models;

namespace TaskAppAPI.Contexts
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<RoleModel> Roles { get; set; }
    }
}
