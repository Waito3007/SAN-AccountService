using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.Auth;

public class LoginRequest
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ResponseType Validate()
    {
        if (string.IsNullOrWhiteSpace(EmailOrUsername))
            return ResponseType.EmailOrUsernameRequired;

        if (string.IsNullOrWhiteSpace(Password))
            return ResponseType.PasswordRequired;

        return ResponseType.Success;
    }
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

