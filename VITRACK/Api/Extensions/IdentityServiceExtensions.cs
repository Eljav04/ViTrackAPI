using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VITRACK.Infrastructure.Data;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Api.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
            options.Password.RequiredUniqueChars = 0;

            // Token providers
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;

            // Lockout settings (optional)
            // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            // options.Lockout.MaxFailedAccessAttempts = 10;
        });

        return services;
    }
}
