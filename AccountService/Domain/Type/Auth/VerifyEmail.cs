using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.Auth;

public class VerifyEmailRequest
{
    public string Token { get; set; } = string.Empty;

    public ResponseType Validate()
    {
        if (string.IsNullOrWhiteSpace(Token))
            return ResponseType.TokenRequired;

        return ResponseType.Success;
    }
}

