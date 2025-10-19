namespace AccountService.Application.Common.Authorization;

/// <summary>
/// Tên các Permission theo format Resource.Action
/// </summary>
public static class PermissionNames
{
    // User Permissions
    public const string UsersRead = "Users.Read";
    public const string UsersCreate = "Users.Create";
    public const string UsersUpdate = "Users.Update";
    public const string UsersSoftDelete = "Users.SoftDelete";
    public const string UsersRestore = "Users.Restore";
    public const string UsersActivate = "Users.Activate";
    public const string UsersDeactivate = "Users.Deactivate";
    public const string UsersLock = "Users.Lock";
    public const string UsersUnlock = "Users.Unlock";

    // Role Permissions
    public const string RolesRead = "Roles.Read";
    public const string RolesCreate = "Roles.Create";
    public const string RolesUpdate = "Roles.Update";
    public const string RolesDelete = "Roles.Delete";
    public const string RolesAssignToUser = "Roles.AssignToUser";
    public const string RolesRemoveFromUser = "Roles.RemoveFromUser";

    // Permission Permissions
    public const string PermissionsRead = "Permissions.Read";
    public const string PermissionsAssignToRole = "Permissions.AssignToRole";
    public const string PermissionsRemoveFromRole = "Permissions.RemoveFromRole";

    // Audit Permissions
    public const string AuditRead = "Audit.Read";
    public const string AuditReadUserTrail = "Audit.ReadUserTrail";

    // Profile Permissions
    public const string ProfilesReadOwn = "Profiles.ReadOwn";
    public const string ProfilesUpdateOwn = "Profiles.UpdateOwn";
    public const string ProfilesReadAny = "Profiles.ReadAny";
    public const string ProfilesUpdateAny = "Profiles.UpdateAny";
}