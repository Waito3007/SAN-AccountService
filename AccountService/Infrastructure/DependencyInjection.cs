using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AccountService.Application.Common.Interfaces;
using AccountService.Infrastructure.Persistence;
using AccountService.Infrastructure.Persistence.Interceptors;
using AccountService.Infrastructure.Services;

namespace AccountService.Infrastructure;

/// <summary>
/// Extension methods để đăng ký Infrastructure layer services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext với Interceptors
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var interceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();
            
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            
            options.AddInterceptors(interceptor);
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // HttpContextAccessor
        services.AddHttpContextAccessor();

        return services;
    }
}

