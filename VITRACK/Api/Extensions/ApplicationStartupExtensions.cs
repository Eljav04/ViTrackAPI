using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VITRACK.Infrastructure.Data;
using VITRACK.Infrastructure.Entities;

public static class ApplicationStartupExtensions
{
    /// <summary>
    /// Checks for and applies any pending database migrations.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var dbContext = services.GetRequiredService<AppDbContext>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                await dbContext.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }

    /// <summary>
    /// Seeds the default Identity roles if they do not exist.
    /// </summary>
    public static async Task AppendSeedRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { Roles.Admin, Roles.User };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();
            logger.LogError(ex, "An error occurred while seeding identity roles.");
            throw;
        }
    }
}