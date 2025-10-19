using AccountService.Domain.Common;
using AccountService.Domain.Enums;

namespace AccountService.Domain.Entities;

/// <summary>
/// Entity User - Thông tin tài khoản người dùng
/// </summary>
public class User : AuditableEntity, ISoftDelete
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserStatus Status { get; set; }
    public AccountType AccountType { get; set; }
    public AuthProvider AuthProvider { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LockedUntil { get; set; }
    public int FailedLoginAttempts { get; set; }

    // Soft Delete Properties
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }
    public string? DeletedReason { get; set; }

    // Navigation Properties
    public UserProfile? Profile { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}