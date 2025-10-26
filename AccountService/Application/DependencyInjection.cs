using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using AccountService.Application.Features.Users.Services;

namespace AccountService.Application;

/// <summary>
/// Đăng ký các service thuộc Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký FluentValidation và các thành phần trong Application layer.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
