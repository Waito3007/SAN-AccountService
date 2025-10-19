using Microsoft.EntityFrameworkCore;
using AccountService.Application.Common.Interfaces;
using AccountService.Domain.Entities;

namespace AccountService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Permission Repository Implementation
/// </summary>
public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(DbContext context) : base(context)
    {
    }

    public async Task<Permission?> GetByCodeAsync(int code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.RolePermissions.Any(rp => rp.RoleId == roleId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.RolePermissions.Any(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId)))
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<int>> GetPermissionCodesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.RolePermissions.Any(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId)))
            .Select(p => p.Code)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsPermissionCodeExistsAsync(int code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(p => p.Code == code, cancellationToken);
    }
}

