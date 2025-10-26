using AccountService.Application.Common.Interfaces;
using AccountService.Infrastructure.Persistence;
using AccountService.Infrastructure.Persistence.Interceptors;
using AccountService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Infrastructure;

/// <summary>
/// Đăng ký các service thuộc Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký DbContext, UnitOfWork và các service hạ tầng khác.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<SoftDeleteInterceptor>();

        services.AddDbContext<ApplicationDbContext>((provider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
            }

            options.UseNpgsql(connectionString);
            options.AddInterceptors(provider.GetRequiredService<SoftDeleteInterceptor>());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

