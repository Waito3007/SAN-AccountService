using AccountService.Domain.Entities;
using AccountService.Domain.Enums;
using AccountService.Application.Common.Models;

namespace AccountService.Application.Common.Interfaces;

/// <summary>
/// AuditLog Repository Interface
/// </summary>
public interface IAuditLogRepository : IGenericRepository<AuditLog>
{
    Task<PaginatedList<AuditLog>> GetAuditLogsByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PaginatedList<AuditLog>> GetAuditLogsByActorIdAsync(Guid actorId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetAuditLogsByActionAsync(AuditAction action, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}


