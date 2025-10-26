using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.Auth;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;

    public ResponseType Validate()
    {
        if (string.IsNullOrWhiteSpace(RefreshToken))
            return ResponseType.RefreshTokenRequired;

        return ResponseType.Success;
    }
}

