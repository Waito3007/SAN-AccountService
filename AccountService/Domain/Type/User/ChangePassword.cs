using AccountService.Domain.Enums;

using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.User;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public ResponseType Validate()
    {
        if (string.IsNullOrWhiteSpace(CurrentPassword))
            return ResponseType.CurrentPasswordRequired;

        if (string.IsNullOrWhiteSpace(NewPassword))
            return ResponseType.NewPasswordRequired;

        if (NewPassword.Length < 6)
            return ResponseType.PasswordTooShort;

        if (NewPassword != ConfirmPassword)
            return ResponseType.PasswordNotMatch;

        if (CurrentPassword == NewPassword)
            return ResponseType.NewPasswordSameAsOld;

        return ResponseType.Success;
    }
}

