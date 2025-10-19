using AccountService.Domain.Entities;

namespace AccountService.Application.Common.Interfaces;

/// <summary>
/// RefreshToken Repository Interface
/// </summary>
public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string token, string revokedByIp, string? reason = null, CancellationToken cancellationToken = default);
    Task<int> RemoveExpiredTokensAsync(CancellationToken cancellationToken = default);
}

