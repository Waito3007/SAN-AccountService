using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountService.Application.Common.Constants;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Application.Features.Users.Dtos;
using AccountService.Application.Features.Users.Mappings;
using AccountService.Application.Features.Users.Services.Requests;
using AccountService.Domain.Entities;
using AccountService.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Features.Users.Services;

/// <summary>
/// Triển khai IUserService, chứa toàn bộ nghiệp vụ người dùng.
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly ILogger<UserService> _logger;
    private readonly IValidator<CreateUserServiceRequest> _createValidator;
    private readonly IValidator<UpdateUserServiceRequest> _updateValidator;
    private readonly IValidator<GetUserByIdServiceRequest> _getByIdValidator;
    private readonly IValidator<GetUsersServiceRequest> _getUsersValidator;
    private readonly IValidator<SoftDeleteUserServiceRequest> _softDeleteValidator;
    private readonly IValidator<RestoreUserServiceRequest> _restoreValidator;
    private readonly IValidator<GetUserAuditTrailServiceRequest> _auditTrailValidator;

    public UserService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService,
        IDateTime dateTime,
        ILogger<UserService> logger,
        IValidator<CreateUserServiceRequest> createValidator,
        IValidator<UpdateUserServiceRequest> updateValidator,
        IValidator<GetUserByIdServiceRequest> getByIdValidator,
        IValidator<GetUsersServiceRequest> getUsersValidator,
        IValidator<SoftDeleteUserServiceRequest> softDeleteValidator,
        IValidator<RestoreUserServiceRequest> restoreValidator,
        IValidator<GetUserAuditTrailServiceRequest> auditTrailValidator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getByIdValidator = getByIdValidator;
        _getUsersValidator = getUsersValidator;
        _softDeleteValidator = softDeleteValidator;
        _restoreValidator = restoreValidator;
        _auditTrailValidator = auditTrailValidator;
    }

    public async Task<Result<UserDto>> CreateAsync(CreateUserServiceRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<UserDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUsername = request.Username.Trim();

        if (await _unitOfWork.Users.IsEmailExistsAsync(normalizedEmail, cancellationToken))
        {
            return Result<UserDto>.Failure(ErrorMessages.EmailAlreadyExists);
        }

        if (await _unitOfWork.Users.IsUsernameExistsAsync(normalizedUsername, cancellationToken))
        {
            return Result<UserDto>.Failure(ErrorMessages.UsernameAlreadyExists);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = BuildNewUser(request, normalizedUsername, normalizedEmail);

            if (request.RoleIds?.Any() == true)
            {
                var roleAssignmentError = await AssignRolesAsync(user, request.RoleIds, cancellationToken);
                if (roleAssignmentError != null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<UserDto>.Failure(roleAssignmentError);
                }
            }

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var createdUser = await _unitOfWork.Users.GetUserDetailAsync(user.Id, cancellationToken) ?? user;

            _logger.LogInformation("User {UserId} created by {Actor}", user.Id, _currentUserService.UserId);

            return Result<UserDto>.Success(createdUser.ToDto(), SuccessMessages.UserCreated);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Create user failed for email {Email}", request.Email);
            return Result<UserDto>.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<UserDto>> UpdateAsync(UpdateUserServiceRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<UserDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var user = await _unitOfWork.Users.GetUserDetailAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UserDto>.Failure(ErrorMessages.UserNotFound);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUsername = request.Username.Trim();

        if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase) &&
            await _unitOfWork.Users.IsEmailExistsAsync(normalizedEmail, cancellationToken))
        {
            return Result<UserDto>.Failure(ErrorMessages.EmailAlreadyExists);
        }

        if (!string.Equals(user.Username, normalizedUsername, StringComparison.OrdinalIgnoreCase) &&
            await _unitOfWork.Users.IsUsernameExistsAsync(normalizedUsername, cancellationToken))
        {
            return Result<UserDto>.Failure(ErrorMessages.UsernameAlreadyExists);
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

            UpdateUserProfile(user, request.Profile);

            var roleUpdateError = await UpdateUserRolesAsync(user, request.RoleIds, cancellationToken);
            if (!string.IsNullOrEmpty(roleUpdateError))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<UserDto>.Failure(roleUpdateError);
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var updatedUser = await _unitOfWork.Users.GetUserDetailAsync(user.Id, cancellationToken) ?? user;

            _logger.LogInformation("User {UserId} updated by {Actor}", user.Id, _currentUserService.UserId);

            return Result<UserDto>.Success(updatedUser.ToDto(), SuccessMessages.UserUpdated);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Update user failed for {UserId}", request.UserId);
            return Result<UserDto>.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<UserDto>> GetByIdAsync(GetUserByIdServiceRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _getByIdValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<UserDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var user = await _unitOfWork.Users.GetUserDetailAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found when querying detail.", request.UserId);
            return Result<UserDto>.Failure(ErrorMessages.UserNotFound);
        }

        return Result<UserDto>.Success(user.ToDto(), SuccessMessages.OperationSuccessful);
    }

    public async Task<Result<PaginatedList<UserDto>>> GetListAsync(GetUsersServiceRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _getUsersValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PaginatedList<UserDto>>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var users = await _unitOfWork.Users.GetUsersWithPaginationAsync(request.PageNumber, request.PageSize, cancellationToken);
        var dto = users.ToDto();

        return Result<PaginatedList<UserDto>>.Success(dto, SuccessMessages.OperationSuccessful);
    }

    public async Task<Result> SoftDeleteAsync(SoftDeleteUserServiceRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _softDeleteValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Soft delete user failed. User {UserId} not found.", request.UserId);
            return Result.Failure(ErrorMessages.UserNotFound);
        }

        if (user.IsDeleted)
        {
            _logger.LogWarning("Soft delete user skipped. User {UserId} already deleted.", request.UserId);
            return Result.Failure(ErrorMessages.AlreadyDeleted);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var reason = request.Reason.Trim();

            user.IsDeleted = true;
            user.DeletedAt = _dateTime.UtcNow;
            user.DeletedByUserId = _currentUserService.UserId;
            user.DeletedReason = reason;
            user.Status = UserStatus.Inactive;

            _unitOfWork.Users.Update(user);

            await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(user.Id, cancellationToken);

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                ActorUserId = _currentUserService.UserId ?? Guid.Empty,
                TargetUserId = user.Id,
                Action = AuditAction.SoftDelete,
                Reason = reason,
                CreatedAt = _dateTime.UtcNow,
                CorrelationId = Guid.NewGuid()
            };

            await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("User {UserId} soft deleted by {ActorUserId}.", user.Id, auditLog.ActorUserId);

            return Result.Success(SuccessMessages.UserDeleted);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Soft delete user failed for {UserId}.", request.UserId);
            return Result.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> RestoreAsync(RestoreUserServiceRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _restoreValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Restore user failed. User {UserId} not found.", request.UserId);
            return Result.Failure(ErrorMessages.UserNotFound);
        }

        if (!user.IsDeleted)
        {
            _logger.LogWarning("Restore user failed. User {UserId} is not deleted.", request.UserId);
            return Result.Failure(ErrorMessages.CannotRestoreNonDeletedUser);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            user.IsDeleted = false;
            user.DeletedAt = null;
            user.DeletedByUserId = null;
            user.DeletedReason = null;
            user.Status = UserStatus.Active;

            _unitOfWork.Users.Update(user);

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                ActorUserId = _currentUserService.UserId ?? Guid.Empty,
                TargetUserId = user.Id,
                Action = AuditAction.Restore,
                CreatedAt = _dateTime.UtcNow,
                CorrelationId = Guid.NewGuid()
            };

            await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("User {UserId} restored by {ActorUserId}.", user.Id, auditLog.ActorUserId);

            return Result.Success(SuccessMessages.UserRestored);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Restore user failed for {UserId}.", request.UserId);
            return Result.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<PaginatedList<AuditLogDto>>> GetAuditTrailAsync(GetUserAuditTrailServiceRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _auditTrailValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return Result<PaginatedList<AuditLogDto>>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Get audit trail failed. User {UserId} not found.", request.UserId);
            return Result<PaginatedList<AuditLogDto>>.Failure(ErrorMessages.UserNotFound);
        }

        var auditLogs = await _unitOfWork.AuditLogs.GetAuditLogsByUserIdAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = auditLogs.Items
            .Select(log => new AuditLogDto
            {
                Id = log.Id,
                ActorUserId = log.ActorUserId,
                ActorUserName = log.Actor?.Username ?? string.Empty,
                TargetUserId = log.TargetUserId,
                Action = log.Action,
                ActionName = log.Action.ToString(),
                Reason = log.Reason,
                Metadata = log.Metadata,
                CreatedAt = log.CreatedAt
            })
            .ToList();

        var result = new PaginatedList<AuditLogDto>(dtos, auditLogs.TotalCount, auditLogs.PageNumber, auditLogs.PageSize);

        return Result<PaginatedList<AuditLogDto>>.Success(result, SuccessMessages.OperationSuccessful);
    }

    private User BuildNewUser(CreateUserServiceRequest request, string username, string email)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber?.Trim(),
            Status = request.Status,
            AccountType = request.AccountType,
            AuthProvider = AuthProvider.Local,
            EmailVerified = false,
            PhoneVerified = false,
            TwoFactorEnabled = false,
            CreatedAt = _dateTime.UtcNow,
            CreatedBy = _currentUserService.UserName,
            LastModifiedAt = _dateTime.UtcNow,
            LastModifiedBy = _currentUserService.UserName,
            UserRoles = new List<UserRole>()
        };

        if (request.Profile != null)
        {
            user.Profile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FirstName = request.Profile.FirstName,
                LastName = request.Profile.LastName,
                MiddleName = request.Profile.MiddleName,
                DisplayName = request.Profile.DisplayName,
                DateOfBirth = request.Profile.DateOfBirth,
                Gender = request.Profile.Gender,
                Avatar = request.Profile.Avatar,
                Bio = request.Profile.Bio,
                AddressLine1 = request.Profile.AddressLine1,
                AddressLine2 = request.Profile.AddressLine2,
                City = request.Profile.City,
                State = request.Profile.State,
                Country = request.Profile.Country,
                PostalCode = request.Profile.PostalCode,
                Timezone = request.Profile.Timezone,
                Language = request.Profile.Language,
                CreatedAt = _dateTime.UtcNow,
                CreatedBy = _currentUserService.UserName
            };
        }

        return user;
    }

    private async Task<string?> AssignRolesAsync(User user, IEnumerable<Guid> requestedRoleIds, CancellationToken cancellationToken)
    {
        var normalizedRoleIds = requestedRoleIds.Distinct().ToList();
        var roles = await _unitOfWork.Roles.FindAsync(r => normalizedRoleIds.Contains(r.Id), cancellationToken);
        var rolesList = roles.ToList();
        var foundRoleIds = rolesList.Select(r => r.Id).ToHashSet();
        var missingRoleId = normalizedRoleIds.FirstOrDefault(id => !foundRoleIds.Contains(id));

        if (missingRoleId != Guid.Empty)
        {
            return ErrorMessages.NotFoundWithId("Role", missingRoleId);
        }

        user.UserRoles = rolesList.Select(role => new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            AssignedAt = _dateTime.UtcNow,
            AssignedByUserId = _currentUserService.UserId,
            Role = role
        }).ToList();

        return null;
    }

    private void UpdateUserProfile(User user, UpdateUserProfileServiceRequest? profileRequest)
    {
        if (profileRequest == null)
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
                CreatedBy = _currentUserService.UserName
            };
        }

        user.Profile.FirstName = profileRequest.FirstName;
        user.Profile.LastName = profileRequest.LastName;
        user.Profile.MiddleName = profileRequest.MiddleName;
        user.Profile.DisplayName = profileRequest.DisplayName;
        user.Profile.DateOfBirth = profileRequest.DateOfBirth;
        user.Profile.Gender = profileRequest.Gender;
        user.Profile.Avatar = profileRequest.Avatar;
        user.Profile.Bio = profileRequest.Bio;
        user.Profile.AddressLine1 = profileRequest.AddressLine1;
        user.Profile.AddressLine2 = profileRequest.AddressLine2;
        user.Profile.City = profileRequest.City;
        user.Profile.State = profileRequest.State;
        user.Profile.Country = profileRequest.Country;
        user.Profile.PostalCode = profileRequest.PostalCode;
        user.Profile.Timezone = profileRequest.Timezone;
        user.Profile.Language = profileRequest.Language;
        user.Profile.LastModifiedAt = _dateTime.UtcNow;
        user.Profile.LastModifiedBy = _currentUserService.UserName;
    }

    private async Task<string?> UpdateUserRolesAsync(User user, IEnumerable<Guid>? requestedRoleIds, CancellationToken cancellationToken)
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
