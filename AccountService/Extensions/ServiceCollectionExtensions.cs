using AccountService.Application.Common.Authorization;
using AccountService.Authorization;
using AccountService.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Extensions;

/// <summary>
/// Extension methods cho cấu hình services.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.RegisterPermissionPolicies();
        });

        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }

    public static IServiceCollection AddCustomMiddlewares(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        return services;
    }
}

