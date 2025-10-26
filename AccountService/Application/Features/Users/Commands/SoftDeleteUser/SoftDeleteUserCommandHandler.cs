using AccountService.Application.Common.Constants;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Domain.Entities;
using AccountService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Features.Users.Commands.SoftDeleteUser;

/// <summary>
/// Xử lý soft delete người dùng.
/// </summary>
public class SoftDeleteUserCommandHandler : IRequestHandler<SoftDeleteUserCommand, ServiceResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly ILogger<SoftDeleteUserCommandHandler> _logger;

    public SoftDeleteUserCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IDateTime dateTime,
        ILogger<SoftDeleteUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<ServiceResponse> Handle(SoftDeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Soft delete user failed. User {UserId} not found.", request.UserId);
            return ServiceResponse.Failure(ErrorMessages.UserNotFound);
        }

        if (user.IsDeleted)
        {
            _logger.LogWarning("Soft delete user skipped. User {UserId} already deleted.", request.UserId);
            return ServiceResponse.Failure(ErrorMessages.AlreadyDeleted);
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

            return ServiceResponse.Success(SuccessMessages.UserDeleted);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Soft delete user failed for {UserId}.", request.UserId);
            return ServiceResponse.Failure(ErrorMessages.OperationFailed);
        }
    }
}

