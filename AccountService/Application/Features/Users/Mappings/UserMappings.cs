using System.Linq;
using AccountService.Application.Common.Models;
using AccountService.Application.Features.Users.Dtos;
using AccountService.Domain.Entities;

namespace AccountService.Application.Features.Users.Mappings;

/// <summary>
/// Extension methods để chuyển đổi entity User sang DTO.
/// </summary>
public static class UserMappings
{
    public static UserDto ToDto(this User user)
    {
        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Status = user.Status,
            AccountType = user.AccountType,
            AuthProvider = user.AuthProvider,
            EmailVerified = user.EmailVerified,
            PhoneVerified = user.PhoneVerified,
            TwoFactorEnabled = user.TwoFactorEnabled,
            LastLoginAt = user.LastLoginAt,
            LockedUntil = user.LockedUntil,
            FailedLoginAttempts = user.FailedLoginAttempts,
            IsDeleted = user.IsDeleted,
            DeletedReason = user.DeletedReason,
            DeletedAt = user.DeletedAt,
            Profile = user.Profile != null
                ? new UserProfileDto
                {
                    FirstName = user.Profile.FirstName,
                    LastName = user.Profile.LastName,
                    MiddleName = user.Profile.MiddleName,
                    DisplayName = user.Profile.DisplayName,
                    DateOfBirth = user.Profile.DateOfBirth,
                    Gender = user.Profile.Gender,
                    Avatar = user.Profile.Avatar,
                    Bio = user.Profile.Bio,
                    AddressLine1 = user.Profile.AddressLine1,
                    AddressLine2 = user.Profile.AddressLine2,
                    City = user.Profile.City,
                    State = user.Profile.State,
                    Country = user.Profile.Country,
                    PostalCode = user.Profile.PostalCode,
                    Timezone = user.Profile.Timezone,
                    Language = user.Profile.Language
                }
                : null,
            Roles = user.UserRoles
                .Select(ur => new RoleDto
                {
                    Id = ur.RoleId,
                    Code = ur.Role?.Code ?? string.Empty,
                    Name = ur.Role?.Name ?? string.Empty
                })
                .OrderBy(r => r.Name)
                .ToList()
        };

        return dto;
    }

    public static PaginatedList<UserDto> ToDto(this PaginatedList<User> paginated)
    {
        var items = paginated.Items
            .Select(u => u.ToDto())
            .ToList();

        return new PaginatedList<UserDto>(items, paginated.TotalCount, paginated.PageNumber, paginated.PageSize);
    }
}
