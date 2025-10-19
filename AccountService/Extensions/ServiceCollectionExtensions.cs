using Microsoft.AspNetCore.Authorization;
using AccountService.Application.Common.Authorization;
using AccountService.Authorization;
using AccountService.Middlewares;

namespace AccountService.Extensions;

/// <summary>
/// Extension methods cho cấu hình services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Đăng ký tất cả permission policies
            options.RegisterPermissionPolicies();
        });

        // Đăng ký Authorization Handler
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }

    public static IServiceCollection AddCustomMiddlewares(this IServiceCollection services)
    {
        // Đăng ký các services cần thiết cho middleware
        services.AddHttpContextAccessor();
        
        return services;
    }
}

