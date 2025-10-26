using System.Collections.Generic;
using AccountService.Domain.Enums;

namespace AccountService.Application.Features.Users.Dtos;

/// <summary>
/// Dto mô tả thông tin người dùng trả về cho client.
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
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
    public bool IsDeleted { get; set; }
    public string? DeletedReason { get; set; }
    public DateTime? DeletedAt { get; set; }
    public UserProfileDto? Profile { get; set; }
    public List<RoleDto> Roles { get; set; } = new();
}
