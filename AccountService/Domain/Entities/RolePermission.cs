namespace AccountService.Domain.Entities;

/// <summary>
/// Entity RolePermission - Bảng trung gian Many-to-Many giữa Role và Permission
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTime AssignedAt { get; set; }
    public Guid? AssignedByUserId { get; set; }

    // Navigation Properties
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

