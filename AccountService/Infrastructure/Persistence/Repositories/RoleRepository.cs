using Microsoft.EntityFrameworkCore;
using AccountService.Application.Common.Interfaces;
using AccountService.Domain.Entities;

namespace AccountService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Role Repository Implementation
/// </summary>
public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(DbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.Code == code, cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<Role?> GetRoleWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.UserRoles.Any(ur => ur.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsRoleCodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(r => r.Code == code, cancellationToken);
    }
}

