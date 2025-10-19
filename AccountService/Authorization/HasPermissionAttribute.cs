using Microsoft.AspNetCore.Authorization;
using AccountService.Domain.Enums;

namespace AccountService.Authorization;

/// <summary>
/// Attribute để kiểm tra Permission cho API endpoint
/// Sử dụng: [HasPermission(PermissionCode.SoftDeleteUser)]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(PermissionCode permissionCode)
    {
        Policy = $"PERM:{(int)permissionCode}";
    }
}