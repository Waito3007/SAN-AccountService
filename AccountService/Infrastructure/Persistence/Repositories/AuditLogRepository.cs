using Microsoft.EntityFrameworkCore;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Domain.Entities;
using AccountService.Domain.Enums;

namespace AccountService.Infrastructure.Persistence.Repositories;

/// <summary>
/// AuditLog Repository Implementation
/// </summary>
public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(DbContext context) : base(context)
    {
    }

    public async Task<PaginatedList<AuditLog>> GetAuditLogsByUserIdAsync(
        Guid userId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(al => al.Actor)
            .Where(al => al.TargetUserId == userId)
            .OrderByDescending(al => al.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<AuditLog>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PaginatedList<AuditLog>> GetAuditLogsByActorIdAsync(
        Guid actorId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(al => al.Actor)
            .Where(al => al.ActorUserId == actorId)
            .OrderByDescending(al => al.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<AuditLog>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsByActionAsync(
        AuditAction action, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(al => al.Action == action)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(al => al.CreatedAt >= startDate && al.CreatedAt <= endDate)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}



