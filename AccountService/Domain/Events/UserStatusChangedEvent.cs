using AccountService.Domain.Enums;
namespace AccountService.Domain.Events;

/// <summary>
/// Domain Event: Trạng thái User thay đổi
/// </summary>
public class UserStatusChangedEvent
{
    public Guid UserId { get; set; }
    public UserStatus OldStatus { get; set; }
    public UserStatus NewStatus { get; set; }
    public Guid ChangedByUserId { get; set; }
    public DateTime ChangedAt { get; set; }
}


