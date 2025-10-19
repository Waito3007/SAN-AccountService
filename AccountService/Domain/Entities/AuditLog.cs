using AccountService.Domain.Common;
using AccountService.Domain.Enums;

namespace AccountService.Domain.Entities;

/// <summary>
/// Entity AuditLog - Ghi nhận các hành động của người dùng
/// </summary>
public class AuditLog : BaseEntity
{
    public Guid ActorUserId { get; set; }
    public Guid? TargetUserId { get; set; }
    public AuditAction Action { get; set; }
    public string? Reason { get; set; }
    public string? Metadata { get; set; } // JSON string
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }

    // Navigation Properties
    public User? Actor { get; set; }
    public User? Target { get; set; }
}
