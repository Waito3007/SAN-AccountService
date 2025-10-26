using System.ComponentModel.DataAnnotations;
using AccountService.Domain.Enums;

namespace AccountService.Contracts.Users;

/// <summary>
/// Thông tin hồ sơ gửi từ client.
/// </summary>
public class UserProfileRequest
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(100)]
    public string? MiddleName { get; set; }

    [MaxLength(100)]
    public string? DisplayName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    [MaxLength(255)]
    public string? Avatar { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }

    [MaxLength(255)]
    public string? AddressLine1 { get; set; }

    [MaxLength(255)]
    public string? AddressLine2 { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(100)]
    public string? Timezone { get; set; }

    [MaxLength(50)]
    public string? Language { get; set; }
}
