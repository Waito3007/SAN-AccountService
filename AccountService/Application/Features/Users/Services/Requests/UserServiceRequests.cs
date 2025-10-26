using System;
using System.Collections.Generic;
using AccountService.Domain.Enums;

namespace AccountService.Application.Features.Users.Services.Requests;

/// <summary>
/// Request tạo mới người dùng cho service layer.
/// </summary>
public record CreateUserServiceRequest
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public AccountType AccountType { get; init; } = AccountType.Customer;
    public UserStatus Status { get; init; } = UserStatus.Active;
    public IReadOnlyCollection<Guid>? RoleIds { get; init; }
    public CreateUserProfileServiceRequest? Profile { get; init; }
}

/// <summary>
/// Thông tin hồ sơ khi tạo người dùng.
/// </summary>
public record CreateUserProfileServiceRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public string? DisplayName { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public Gender Gender { get; init; }
    public string? Avatar { get; init; }
    public string? Bio { get; init; }
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Country { get; init; }
    public string? PostalCode { get; init; }
    public string? Timezone { get; init; }
    public string? Language { get; init; }
}

/// <summary>
/// Request cập nhật người dùng cho service layer.
/// </summary>
public record UpdateUserServiceRequest
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public UserStatus Status { get; init; }
    public AccountType AccountType { get; init; }
    public bool EmailVerified { get; init; }
    public bool PhoneVerified { get; init; }
    public bool TwoFactorEnabled { get; init; }
    public IReadOnlyCollection<Guid>? RoleIds { get; init; }
    public UpdateUserProfileServiceRequest? Profile { get; init; }
}

/// <summary>
/// Thông tin hồ sơ khi cập nhật người dùng.
/// </summary>
public record UpdateUserProfileServiceRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public string? DisplayName { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public Gender Gender { get; init; }
    public string? Avatar { get; init; }
    public string? Bio { get; init; }
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Country { get; init; }
    public string? PostalCode { get; init; }
    public string? Timezone { get; init; }
    public string? Language { get; init; }
}

/// <summary>
/// Request lấy thông tin người dùng theo Id.
/// </summary>
public record GetUserByIdServiceRequest
{
    public Guid UserId { get; init; }
}

/// <summary>
/// Request lấy danh sách người dùng phân trang.
/// </summary>
public record GetUsersServiceRequest
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}

/// <summary>
/// Request soft delete người dùng.
/// </summary>
public record SoftDeleteUserServiceRequest
{
    public Guid UserId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Request khôi phục người dùng đã bị soft delete.
/// </summary>
public record RestoreUserServiceRequest
{
    public Guid UserId { get; init; }
}

/// <summary>
/// Request lấy audit trail của người dùng.
/// </summary>
public record GetUserAuditTrailServiceRequest
{
    public Guid UserId { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}
