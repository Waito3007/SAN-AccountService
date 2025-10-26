using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using AccountService.Application.Services;
using AccountService.Application.Services.Interfaces;

namespace AccountService.Application;

/// <summary>
/// Extension methods để đăng ký Application layer services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAuthService, AuthService>();

        // MediatR (chỉ cho Domain Events)
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });


        return services;
    }
}