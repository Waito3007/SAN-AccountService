using System.Collections.Generic;
using System.Linq;
using AccountService.Application.Common.Constants;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Application.Features.Users.Dtos;
using AccountService.Application.Features.Users.Mappings;
using AccountService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Features.Users.Commands.UpdateUser;

/// <summary>
/// Handler cập nhật thông tin người dùng.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ServiceResponse<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IDateTime dateTime,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<ServiceResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserDetailAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return ServiceResponse<UserDto>.Failure(ErrorMessages.UserNotFound);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUsername = request.Username.Trim();

        if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase) &&
            await _unitOfWork.Users.IsEmailExistsAsync(normalizedEmail, cancellationToken))
        {
            return ServiceResponse<UserDto>.Failure(ErrorMessages.EmailAlreadyExists);
        }

        if (!string.Equals(user.Username, normalizedUsername, StringComparison.OrdinalIgnoreCase) &&
            await _unitOfWork.Users.IsUsernameExistsAsync(normalizedUsername, cancellationToken))
        {
            return ServiceResponse<UserDto>.Failure(ErrorMessages.UsernameAlreadyExists);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            user.Username = normalizedUsername;
            user.Email = normalizedEmail;
            user.PhoneNumber = request.PhoneNumber?.Trim();
            user.Status = request.Status;
            user.AccountType = request.AccountType;
            user.EmailVerified = request.EmailVerified;
            user.PhoneVerified = request.PhoneVerified;
            user.TwoFactorEnabled = request.TwoFactorEnabled;
            user.LastModifiedAt = _dateTime.UtcNow;
            user.LastModifiedBy = _currentUserService.UserName;

            UpdateUserProfile(user, request);

            var roleUpdateError = await UpdateUserRolesAsync(user, request.RoleIds, cancellationToken);
            if (!string.IsNullOrEmpty(roleUpdateError))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ServiceResponse<UserDto>.Failure(roleUpdateError);
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var updatedUser = await _unitOfWork.Users.GetUserDetailAsync(user.Id, cancellationToken)
                ?? user;

            _logger.LogInformation("User {UserId} updated by {Actor}", user.Id, _currentUserService.UserId);

            return ServiceResponse<UserDto>.Success(updatedUser.ToDto(), SuccessMessages.UserUpdated);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Update user failed for {UserId}", request.UserId);
            return ServiceResponse<UserDto>.Failure(ErrorMessages.OperationFailed);
        }
    }

    private void UpdateUserProfile(User user, UpdateUserCommand request)
    {
        if (request.Profile == null)
        {
            return;
        }

        if (user.Profile == null)
        {
            user.Profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = _dateTime.UtcNow,
                CreatedBy = _currentUserService.UserName,
            };
        }

        user.Profile.FirstName = request.Profile.FirstName;
        user.Profile.LastName = request.Profile.LastName;
        user.Profile.MiddleName = request.Profile.MiddleName;
        user.Profile.DisplayName = request.Profile.DisplayName;
        user.Profile.DateOfBirth = request.Profile.DateOfBirth;
        user.Profile.Gender = request.Profile.Gender;
        user.Profile.Avatar = request.Profile.Avatar;
        user.Profile.Bio = request.Profile.Bio;
        user.Profile.AddressLine1 = request.Profile.AddressLine1;
        user.Profile.AddressLine2 = request.Profile.AddressLine2;
        user.Profile.City = request.Profile.City;
        user.Profile.State = request.Profile.State;
        user.Profile.Country = request.Profile.Country;
        user.Profile.PostalCode = request.Profile.PostalCode;
        user.Profile.Timezone = request.Profile.Timezone;
        user.Profile.Language = request.Profile.Language;
        user.Profile.LastModifiedAt = _dateTime.UtcNow;
        user.Profile.LastModifiedBy = _currentUserService.UserName;
    }

    private async Task<string?> UpdateUserRolesAsync(User user, IEnumerable<Guid> requestedRoleIds, CancellationToken cancellationToken)
    {
        var normalizedRoleIds = requestedRoleIds?.Distinct().ToList() ?? new List<Guid>();
        var currentRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();

        var rolesToAdd = normalizedRoleIds.Except(currentRoleIds).ToList();
        var rolesToRemove = currentRoleIds.Except(normalizedRoleIds).ToList();

        if (!rolesToAdd.Any() && !rolesToRemove.Any())
        {
            return null;
        }

        if (rolesToAdd.Any())
        {
            var roles = await _unitOfWork.Roles.FindAsync(r => rolesToAdd.Contains(r.Id), cancellationToken);
            var rolesList = roles.ToList();
            var foundRoleIds = rolesList.Select(r => r.Id).ToHashSet();
            var missingRoleId = rolesToAdd.FirstOrDefault(id => !foundRoleIds.Contains(id));

            if (missingRoleId != Guid.Empty)
            {
                return ErrorMessages.NotFoundWithId("Role", missingRoleId);
            }

            foreach (var role in rolesList)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    AssignedAt = _dateTime.UtcNow,
                    AssignedByUserId = _currentUserService.UserId,
                    Role = role
                });
            }
        }

        if (rolesToRemove.Any())
        {
            var toRemove = user.UserRoles
                .Where(ur => rolesToRemove.Contains(ur.RoleId))
                .ToList();

            foreach (var userRole in toRemove)
            {
                user.UserRoles.Remove(userRole);
            }
        }

        return null;
    }
}
