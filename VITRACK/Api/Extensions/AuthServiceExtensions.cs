using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VITRACK.Infrastructure.Data;
using VITRACK.Infrastructure.Entities;

namespace VITRACK.Api.Extensions;

public static class AuthServiceExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, string secretKey)
    {

        services.AddAuthentication(a =>
        {
            a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(j =>
        {
            j.RequireHttpsMetadata = false;
            j.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // Attach JWT with Cookies 
            j.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["auth_token"];
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}
