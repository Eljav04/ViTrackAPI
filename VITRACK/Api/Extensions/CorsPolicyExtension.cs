
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
                "https://localhost:5173",
                "http://192.168.10.116:5173",
                "https://192.168.10.116:5173",
                "http://192.168.10.116:5500",
                "https://192.168.10.116:5500",
                "http://paybir-001-site1.jtempurl.com",
                "https://paybir-001-site1.jtempurl.com",
                "http://192.168.1.42:5500",
                "https://192.168.1.42:5500",
                "http://viva-festival-lottery.elvindzhavadov.workers.dev",
                "https://viva-festival-lottery.elvindzhavadov.workers.dev",
                "https://viva-fest-quiz.elvindzhavadov.workers.dev",
                "http://viva-fest-quiz.elvindzhavadov.workers.dev"

                )
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});


        return services;
    }
}
