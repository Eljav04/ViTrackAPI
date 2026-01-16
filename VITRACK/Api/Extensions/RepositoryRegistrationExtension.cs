using Microsoft.Extensions.DependencyInjection;
using VITRACK.Application.Interfaces;
using VITRACK.Application.Repositories;

namespace VITRACK.Api.Extensions;

public static class RepositoryRegistrationExtension
{
    public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        return services;
    }
}
