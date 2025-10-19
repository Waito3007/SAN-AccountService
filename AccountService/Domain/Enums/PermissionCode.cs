namespace AccountService.Domain.Enums;

/// <summary>
/// Mã quyền (Permission Code) - Đơn vị phân quyền nhỏ nhất
/// Mỗi code tương ứng với một permission cụ thể
/// </summary>
public enum PermissionCode
{
    // User Management Permissions
    ReadUser = 1,
    CreateUser = 2,
    UpdateUser = 3,
    SoftDeleteUser = 4,
    RestoreUser = 5,
    ActivateUser = 6,
    DeactivateUser = 7,
    LockUser = 8,
    UnlockUser = 9,

    // Role Management Permissions
    ReadRole = 20,
    CreateRole = 21,
    UpdateRole = 22,
    DeleteRole = 23,
    AssignRoleToUser = 24,
    RemoveRoleFromUser = 25,

    // Permission Management
    ReadPermission = 40,
    AssignPermissionsToRole = 41,
    RemovePermissionsFromRole = 42,

    // Audit Permissions
    ReadAuditLog = 60,
    ReadUserAuditTrail = 61,

    // Profile Permissions
    ReadOwnProfile = 80,
    UpdateOwnProfile = 81,
    ReadAnyProfile = 82,
    UpdateAnyProfile = 83
}

