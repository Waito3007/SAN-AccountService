namespace AccountService.Application.Common.Interfaces;

/// <summary>
/// Service để hash và verify mật khẩu người dùng.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}
