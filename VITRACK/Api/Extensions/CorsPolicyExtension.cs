
namespace VITRACK.Api.Extensions;

public static class CorsPolicyExtension
{
    public static IServiceCollection AddAllowedSpecificOrigins(this IServiceCollection services)
    {
        services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173"
                )
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});


        return services;
    }
}
