using Microsoft.EntityFrameworkCore;
using VITRACK.Addons.Quiz.Data;
using VITRACK.Addons.Quiz.Interfaces;
using VITRACK.Addons.Quiz.Services;

namespace VITRACK.Api.Extensions;

public static class QuizModuleExtensions
{
    public static IServiceCollection AddQuizModule(this IServiceCollection services, IConfiguration configuration)
    {
        var dbPath = Path.Join(Directory.GetCurrentDirectory(), "Addons/Quiz/quiz.db");
        services.AddDbContext<QuizDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped<IQuizService, QuizService>();

        return services;
    }
}
