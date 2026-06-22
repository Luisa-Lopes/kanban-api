using Microsoft.EntityFrameworkCore;
using Tables.Models;


namespace ProjectManager.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Projects> Projects { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
}