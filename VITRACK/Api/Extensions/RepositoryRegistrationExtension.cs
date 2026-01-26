using Microsoft.Extensions.DependencyInjection;
using VITRACK.Application.Interfaces;
using VITRACK.Application.Repositories;
using VITRACK.Common.Services;

namespace VITRACK.Api.Extensions;

public static class RepositoryRegistrationExtension
{
    public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAttendanceRecordRepository, AttendanceRecordRepository>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IStatisticsRepository, StatisticsRepository>();
        return services;
    }
}
