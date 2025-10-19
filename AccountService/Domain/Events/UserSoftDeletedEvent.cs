namespace AccountService.Domain.Events;

/// <summary>
/// Domain Event: User bị soft delete
/// </summary>
public class UserSoftDeletedEvent
{
    public Guid UserId { get; set; }
    public Guid DeletedByUserId { get; set; }
    public string? DeletedReason { get; set; }
    public DateTime DeletedAt { get; set; }
}

