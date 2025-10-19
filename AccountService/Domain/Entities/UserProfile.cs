using AccountService.Domain.Common;
using AccountService.Domain.Enums;

namespace AccountService.Domain.Entities;

/// <summary>
/// Entity UserProfile - Thông tin hồ sơ người dùng
/// </summary>
public class UserProfile : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? DisplayName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Timezone { get; set; }
    public string? Language { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
}

