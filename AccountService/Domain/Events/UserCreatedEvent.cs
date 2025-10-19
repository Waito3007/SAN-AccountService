namespace AccountService.Domain.Events;

/// <summary>
/// Domain Event: User được tạo mới
/// </summary>
public class UserCreatedEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}