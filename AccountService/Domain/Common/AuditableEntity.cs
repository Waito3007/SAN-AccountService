namespace AccountService.Domain.Common;

/// <summary>
/// Base entity có thể audit (theo dõi người tạo/sửa và thời gian)
/// </summary>
public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}

