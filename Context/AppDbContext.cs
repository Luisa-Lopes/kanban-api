using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tables.Models;

namespace ProjectManager.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Projects> Projects { get; set; }
    public DbSet<ProjectMembers> ProjectMembers { get; set; }
    public DbSet<ProjectSprint> ProjectSprint { get; set; }
    public DbSet<ProjectTasks> ProjectTasks { get; set; }
    public DbSet<ProjectInvitations> ProjectInvitation { get; set; }
 
   
}