using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Infrastructure.Data;

public sealed class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<WorkSchedule> WorkSchedules => Set<WorkSchedule>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // === Department Entity Configuration ===
        builder.Entity<Department>()
            .HasMany<User>()
            .WithOne(u => u.Department)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
        // === Department Entity Configuration ===

        // === WorkSchedule Entity Configuration ===
        builder.Entity<WorkSchedule>()
            .HasMany<User>()
            .WithOne(u => u.WorkSchedule)
            .HasForeignKey(u => u.WorkScheduleId)
            .OnDelete(DeleteBehavior.SetNull);
        // === WorkSchedule Entity Configuration ===

    }
}