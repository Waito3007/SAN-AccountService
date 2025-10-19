namespace AccountService.Domain.Entities;

/// <summary>
/// Entity UserRole - Bảng trung gian Many-to-Many giữa User và Role
/// </summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; }
    public Guid? AssignedByUserId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

