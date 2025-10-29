using Microsoft.EntityFrameworkCore;
using Workplace.Tasks.Api.Models;

namespace Workplace.Tasks.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>(b => {
            b.HasKey(t => t.Id);
            b.Property(t => t.Title).IsRequired();
            b.Property(t => t.Description);
            b.Property(t => t.Status).HasConversion<string>().IsRequired();
            b.Property(t => t.CreatedAt).HasDefaultValueSql("now()");
            b.Property(t => t.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<User>(b => {
            b.HasKey(u => u.Id);
            b.Property(u => u.Email).IsRequired();
            b.Property(u => u.PasswordHash).IsRequired();
            b.Property(u => u.Role).IsRequired();
        });
    }
}
