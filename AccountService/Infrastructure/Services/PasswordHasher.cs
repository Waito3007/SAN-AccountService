using System.Security.Cryptography;
using AccountService.Application.Common.Interfaces;

namespace AccountService.Infrastructure.Services;

/// <summary>
/// Triển khai hasher mật khẩu sử dụng PBKDF2.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32;  // 256 bit
    private const int Iterations = 100_000;

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password không được để trống.", nameof(password));
        }

        using var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256);
        var salt = deriveBytes.Salt;
        var key = deriveBytes.GetBytes(KeySize);

        return string.Join('.', Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(providedPassword))
        {
            return false;
        }

        var parts = hashedPassword.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[1]);
        var storedKey = Convert.FromBase64String(parts[2]);

        using var deriveBytes = new Rfc2898DeriveBytes(providedPassword, salt, iterations, HashAlgorithmName.SHA256);
        var computedKey = deriveBytes.GetBytes(storedKey.Length);

        return CryptographicOperations.FixedTimeEquals(storedKey, computedKey);
    }
}
