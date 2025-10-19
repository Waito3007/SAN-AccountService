using Microsoft.AspNetCore.Authorization;
using AccountService.Application.Common.Authorization;
using AccountService.Domain.Enums;

namespace AccountService.Authorization;

/// <summary>
/// Class đăng ký các Policy cho Permission-based authorization
/// </summary>
public static class PermissionPolicies
{
    public static void RegisterPermissionPolicies(this AuthorizationOptions options)
    {
        // Đăng ký policy cho từng permission code
        foreach (PermissionCode permissionCode in Enum.GetValues(typeof(PermissionCode)))
        {
            options.AddPolicy(
                $"PERM:{(int)permissionCode}",
                policy => policy.Requirements.Add(new PermissionRequirement(permissionCode))
            );
        }
    }
}
