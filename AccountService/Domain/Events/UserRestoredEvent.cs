namespace AccountService.Domain.Events;

/// <summary>
/// Domain Event: User được khôi phục từ trạng thái deleted
/// </summary>
public class UserRestoredEvent
{
    public Guid UserId { get; set; }
    public Guid RestoredByUserId { get; set; }
    public DateTime RestoredAt { get; set; }
}

