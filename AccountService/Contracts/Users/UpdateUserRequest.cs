using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccountService.Domain.Enums;

namespace AccountService.Contracts.Users;

/// <summary>
/// Request body cập nhật thông tin người dùng.
/// </summary>
public class UpdateUserRequest
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public UserStatus Status { get; set; }

    public AccountType AccountType { get; set; }

    public bool EmailVerified { get; set; }

    public bool PhoneVerified { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public UserProfileRequest? Profile { get; set; }

    public List<Guid> RoleIds { get; set; } = new();
}
