using AccountService.Domain.Entities;

namespace AccountService.Application.Common.Interfaces;

/// <summary>
/// Permission Repository Interface
/// </summary>
public interface IPermissionRepository : IGenericRepository<Permission>
{
    Task<Permission?> GetByCodeAsync(int code, CancellationToken cancellationToken = default);
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<int>> GetPermissionCodesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsPermissionCodeExistsAsync(int code, CancellationToken cancellationToken = default);
}

