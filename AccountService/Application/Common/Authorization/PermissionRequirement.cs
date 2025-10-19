using Microsoft.AspNetCore.Authorization;
using AccountService.Domain.Enums;

namespace AccountService.Application.Common.Authorization;

/// <summary>
/// Authorization Requirement cho Permission-based authorization
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionCode Code { get; }

    public PermissionRequirement(PermissionCode code)
    {
        Code = code;
    }
}

