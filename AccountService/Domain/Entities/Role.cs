using AccountService.Domain.Common;

namespace AccountService.Domain.Entities;

/// <summary>
/// Entity Role - Vai trò (tập hợp các quyền)
/// </summary>
public class Role : AuditableEntity
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation Properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

