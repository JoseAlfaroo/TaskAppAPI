using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Models;

namespace TaskAppAPI.Contexts
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
    }
}
