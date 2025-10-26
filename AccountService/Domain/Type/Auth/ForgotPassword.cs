using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.Auth;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;

    public ResponseType Validate()
    {
        if (string.IsNullOrWhiteSpace(Email))
            return ResponseType.EmailRequired;

        if (!IsValidEmail(Email))
            return ResponseType.EmailInvalid;

        return ResponseType.Success;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

