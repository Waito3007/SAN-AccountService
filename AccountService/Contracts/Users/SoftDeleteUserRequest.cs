using System.ComponentModel.DataAnnotations;

namespace AccountService.Contracts.Users;

/// <summary>
/// Request DTO cho soft delete người dùng.
/// </summary>
public class SoftDeleteUserRequest
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}
