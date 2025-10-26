using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccountService.Domain.Enums;

namespace AccountService.Contracts.Users;

/// <summary>
/// Request body tạo mới người dùng.
/// </summary>
public class CreateUserRequest
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public AccountType AccountType { get; set; } = AccountType.Customer;

    public UserStatus Status { get; set; } = UserStatus.Active;

    public UserProfileRequest? Profile { get; set; }

    public List<Guid> RoleIds { get; set; } = new();
}
