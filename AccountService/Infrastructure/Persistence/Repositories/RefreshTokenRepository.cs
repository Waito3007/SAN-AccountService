using Microsoft.EntityFrameworkCore;
using AccountService.Application.Common.Interfaces;
using AccountService.Domain.Entities;

namespace AccountService.Infrastructure.Persistence.Repositories;

/// <summary>
/// RefreshToken Repository Implementation
/// </summary>
public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(DbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _dbSet
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.ReasonRevoked = "All tokens revoked";
        }
    }

    public async Task RevokeTokenAsync(string token, string revokedByIp, string? reason = null, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _dbSet
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

        if (refreshToken != null && refreshToken.RevokedAt == null)
        {
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = revokedByIp;
            refreshToken.ReasonRevoked = reason ?? "Token revoked";
        }
    }

    public async Task<int> RemoveExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await _dbSet
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(expiredTokens);
        
        return expiredTokens.Count;
    }
}

