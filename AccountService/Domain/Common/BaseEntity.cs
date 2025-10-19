namespace AccountService.Domain.Common;

/// <summary>
/// Base entity cho tất cả entities trong domain
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
}


