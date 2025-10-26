using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Application;

/// <summary>
/// Đăng ký các service thuộc Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký MediatR, FluentValidation và các thành phần trong Application layer.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}

