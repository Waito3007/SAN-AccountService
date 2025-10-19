using MediatR;
using AccountService.Application.Common.Models;
using AccountService.Domain.Enums;

namespace AccountService.Application.Features.Users.Queries.GetUserAuditTrail;

/// <summary>
/// Query để lấy audit trail của user
/// </summary>
public class GetUserAuditTrailQuery : IRequest<Result<List<AuditLogDto>>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// DTO cho Audit Log
/// </summary>
public class AuditLogDto
{
    public Guid Id { get; set; }
    public Guid ActorUserId { get; set; }
    public string ActorUserName { get; set; } = string.Empty;
    public Guid? TargetUserId { get; set; }
    public AuditAction Action { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
}

