namespace AccountService.Domain.Enums;

/// <summary>
/// Mã quyền (Permission Code) - Đơn vị phân quyền nhỏ nhất
/// Mỗi code tương ứng với một permission cụ thể
/// </summary>
public enum PermissionCode
{
    // User Management Permissions
    UserRead = 1,
    UserCreate = 2,
    UserUpdate = 3,
    UserDelete = 4,
    UserRestore = 5,
    UserLock = 8,
    UserPasswordReset = 10,

    // Role Management Permissions
    RoleRead = 20,
    RoleCreate = 21,
    RoleUpdate = 22,
    RoleDelete = 23,
    RoleAssign = 24,

    // Permission Management
    PermissionRead = 40,
    PermissionAssign = 41,

    // Audit Permissions
    AuditRead = 60,

    // Profile Permissions
    ProfileRead = 80,
    ProfileUpdate = 81
}


