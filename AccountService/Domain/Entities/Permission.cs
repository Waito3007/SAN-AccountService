using AccountService.Domain.Common;

namespace AccountService.Domain.Entities;

/// <summary>
/// Entity Permission - Quyền (đơn vị phân quyền nhỏ nhất)
/// </summary>
public class Permission : BaseEntity
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

