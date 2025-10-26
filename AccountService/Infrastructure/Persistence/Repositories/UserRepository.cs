using System.Linq;
using Microsoft.EntityFrameworkCore;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Domain.Entities;
using AccountService.Domain.Enums;

namespace AccountService.Infrastructure.Persistence.Repositories;

/// <summary>
/// User Repository Implementation
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(DbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetUserWithProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> GetUserWithRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> GetUserWithPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> GetUserDetailAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Profile)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<PaginatedList<User>> GetUsersWithPaginationAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(u => u.Profile)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Username);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<User>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(u => u.Status == UserStatus.Active && !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetDeletedUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .IgnoreQueryFilters() // Bỏ qua global query filter
            .Where(u => u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsUsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Username == username, cancellationToken);
    }
}
