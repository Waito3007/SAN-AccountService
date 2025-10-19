namespace AccountService.Application.Common.Interfaces;

/// <summary>
/// Unit of Work Interface - Quản lý transaction và SaveChanges
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repositories
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IPermissionRepository Permissions { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IAuditLogRepository AuditLogs { get; }

    // Transaction Management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}