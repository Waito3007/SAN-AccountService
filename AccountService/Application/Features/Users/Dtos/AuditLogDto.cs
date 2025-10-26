using System;
using AccountService.Domain.Enums;

namespace AccountService.Application.Features.Users.Dtos;

/// <summary>
/// DTO đại diện cho bản ghi audit của người dùng.
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
