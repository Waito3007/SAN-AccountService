using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.Auth;

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public ResponseType Validate()
    {
        if (string.IsNullOrWhiteSpace(Token))
            return ResponseType.TokenRequired;

        if (string.IsNullOrWhiteSpace(NewPassword))
            return ResponseType.NewPasswordRequired;

        if (NewPassword.Length < 6)
            return ResponseType.PasswordTooShort;

        if (NewPassword != ConfirmPassword)
            return ResponseType.PasswordNotMatch;

        return ResponseType.Success;
    }
}

