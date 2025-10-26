using System.Linq;
using AccountService.Application.Common.Constants;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Application.Features.Users.Dtos;
using AccountService.Application.Features.Users.Mappings;
using AccountService.Domain.Entities;
using AccountService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Handler tạo mới người dùng.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ServiceResponse<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService,
        IDateTime dateTime,
        ILogger<CreateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<ServiceResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUsername = request.Username.Trim();

        if (await _unitOfWork.Users.IsEmailExistsAsync(normalizedEmail, cancellationToken))
        {
            return ServiceResponse<UserDto>.Failure(ErrorMessages.EmailAlreadyExists);
        }

        if (await _unitOfWork.Users.IsUsernameExistsAsync(normalizedUsername, cancellationToken))
        {
            return ServiceResponse<UserDto>.Failure(ErrorMessages.UsernameAlreadyExists);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = normalizedUsername,
                Email = normalizedEmail,
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
                    CreatedBy = _currentUserService.UserName,
                };
            }

            if (request.RoleIds?.Any() == true)
            {
                var requestedRoleIds = request.RoleIds.Distinct().ToList();
                var roles = await _unitOfWork.Roles.FindAsync(r => requestedRoleIds.Contains(r.Id), cancellationToken);
                var rolesList = roles.ToList();
                var foundRoleIds = rolesList.Select(r => r.Id).ToHashSet();
                var missingRoleIds = requestedRoleIds.Where(id => !foundRoleIds.Contains(id)).ToList();

                if (missingRoleIds.Any())
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ServiceResponse<UserDto>.Failure(ErrorMessages.NotFoundWithId("Role", missingRoleIds.First()));
                }

                user.UserRoles = rolesList.Select(role => new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    AssignedAt = _dateTime.UtcNow,
                    AssignedByUserId = _currentUserService.UserId,
                    Role = role
                }).ToList();
            }

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var createdUser = await _unitOfWork.Users.GetUserDetailAsync(user.Id, cancellationToken)
                ?? user;

            _logger.LogInformation("User {UserId} created by {Actor}", user.Id, _currentUserService.UserId);

            return ServiceResponse<UserDto>.Success(createdUser.ToDto(), SuccessMessages.UserCreated);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Create user failed for email {Email}", request.Email);
            return ServiceResponse<UserDto>.Failure(ErrorMessages.OperationFailed);
        }
    }
}
