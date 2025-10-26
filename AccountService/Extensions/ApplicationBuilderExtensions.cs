using AccountService.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace AccountService.Extensions;

/// <summary>
/// Extension methods cho cấu hình application pipeline.
/// </summary>
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}

