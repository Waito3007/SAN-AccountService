namespace AccountService.Domain.Common;

/// <summary>
/// Interface đánh dấu entity hỗ trợ Soft Delete
/// </summary>
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    Guid? DeletedByUserId { get; set; }
    string? DeletedReason { get; set; }
}