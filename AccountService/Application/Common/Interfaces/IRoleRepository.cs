using AccountService.Domain.Entities;

namespace AccountService.Application.Common.Interfaces;

/// <summary>
/// Role Repository Interface
/// </summary>
public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsRoleCodeExistsAsync(string code, CancellationToken cancellationToken = default);
}

