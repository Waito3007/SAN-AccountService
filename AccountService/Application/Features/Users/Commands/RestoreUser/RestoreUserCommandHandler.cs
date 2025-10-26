using AccountService.Application.Common.Constants;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Domain.Entities;
using AccountService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Features.Users.Commands.RestoreUser;

/// <summary>
/// Xử lý khôi phục người dùng đã bị soft delete.
/// </summary>
public class RestoreUserCommandHandler : IRequestHandler<RestoreUserCommand, ServiceResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly ILogger<RestoreUserCommandHandler> _logger;

    public RestoreUserCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IDateTime dateTime,
        ILogger<RestoreUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<ServiceResponse> Handle(RestoreUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users
            .GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Restore user failed. User {UserId} not found.", request.UserId);
            return ServiceResponse.Failure(ErrorMessages.UserNotFound);
        }

        if (!user.IsDeleted)
        {
            _logger.LogWarning("Restore user failed. User {UserId} is not deleted.", request.UserId);
            return ServiceResponse.Failure(ErrorMessages.CannotRestoreNonDeletedUser);
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

            return ServiceResponse.Success(SuccessMessages.UserRestored);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Restore user failed for {UserId}.", request.UserId);
            return ServiceResponse.Failure(ErrorMessages.OperationFailed);
        }
    }
}

