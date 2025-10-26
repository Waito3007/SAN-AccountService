using System.Collections.Generic;
using AccountService.Application.Common.Models;
using AccountService.Application.Features.Users.Dtos;
using AccountService.Domain.Enums;
using MediatR;

namespace AccountService.Application.Features.Users.Commands.UpdateUser;

/// <summary>
/// Command cập nhật thông tin người dùng.
/// </summary>
public class UpdateUserCommand : IRequest<ServiceResponse<UserDto>>
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserStatus Status { get; set; }
    public AccountType AccountType { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public UpdateUserProfileDto? Profile { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}

/// <summary>
/// Thông tin hồ sơ cập nhật.
/// </summary>
public class UpdateUserProfileDto
{
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
}
