using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Constants;
using AccountService.Domain.Exceptions;
using AccountService.Infrastructure.Persistence.Repositories;

namespace AccountService.Infrastructure.Persistence;

/// <summary>
/// Unit of Work Implementation - Quản lý repositories và transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy initialization cho repositories
    private IUserRepository? _users;
    private IRoleRepository? _roles;
    private IPermissionRepository? _permissions;
    private IRefreshTokenRepository? _refreshTokens;
    private IAuditLogRepository? _auditLogs;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Repository Properties với Lazy Loading
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
    public IPermissionRepository Permissions => _permissions ??= new PermissionRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
    public IAuditLogRepository AuditLogs => _auditLogs ??= new AuditLogRepository(_context);

    /// <summary>
    /// Lưu tất cả thay đổi vào database
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Bắt đầu transaction
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new TransactionException(ErrorMessages.TransactionAlreadyStarted);
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commit transaction - Lưu tất cả thay đổi
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new TransactionException(ErrorMessages.NoActiveTransaction);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rollback transaction - Hủy bỏ tất cả thay đổi
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}
