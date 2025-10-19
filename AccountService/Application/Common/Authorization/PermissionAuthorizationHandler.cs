using Microsoft.AspNetCore.Authorization;

namespace AccountService.Application.Common.Authorization;

/// <summary>
/// Authorization Handler kiểm tra Permission từ JWT claims
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        // Lấy claim "perms" từ JWT token
        var permissionsClaim = context.User.FindFirst("perms");
        
        if (permissionsClaim == null || string.IsNullOrEmpty(permissionsClaim.Value))
        {
            return Task.CompletedTask;
        }

        // Parse danh sách permission codes
        var userPermissions = permissionsClaim.Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => int.TryParse(p.Trim(), out var code) ? code : -1)
            .Where(p => p != -1)
            .ToHashSet();

        // Kiểm tra user có permission yêu cầu không
        if (userPermissions.Contains((int)requirement.Code))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
