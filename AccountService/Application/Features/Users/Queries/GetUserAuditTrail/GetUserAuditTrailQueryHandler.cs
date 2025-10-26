using AccountService.Application.Common.Constants;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Features.Users.Queries.GetUserAuditTrail;

/// <summary>
/// Xử lý truy vấn lấy lịch sử audit của người dùng.
/// </summary>
public class GetUserAuditTrailQueryHandler : IRequestHandler<GetUserAuditTrailQuery, ServiceResponse<PaginatedList<AuditLogDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUserAuditTrailQueryHandler> _logger;

    public GetUserAuditTrailQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUserAuditTrailQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ServiceResponse<PaginatedList<AuditLogDto>>> Handle(GetUserAuditTrailQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Get audit trail failed. User {UserId} not found.", request.UserId);
            return ServiceResponse<PaginatedList<AuditLogDto>>.Failure(ErrorMessages.UserNotFound);
        }

        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var auditLogs = await _unitOfWork.AuditLogs
            .GetAuditLogsByUserIdAsync(request.UserId, pageNumber, pageSize, cancellationToken);

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

        return ServiceResponse<PaginatedList<AuditLogDto>>.Success(result, SuccessMessages.OperationSuccessful);
    }
}

