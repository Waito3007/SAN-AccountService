using AccountService.Domain.Entities;
using AccountService.Application.Common.Models;

namespace AccountService.Application.Common.Interfaces;

/// <summary>
/// User Repository Interface - Các phương thức specific cho User
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetUserWithProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserWithPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserDetailAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PaginatedList<User>> GetUsersWithPaginationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetDeletedUsersAsync(CancellationToken cancellationToken = default);
    Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameExistsAsync(string username, CancellationToken cancellationToken = default);
}

