using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.Auth;

public class Verify2FARequest
{
    public string Code { get; set; } = string.Empty;

    public ResponseType Validate()
    {
        if (string.IsNullOrWhiteSpace(Code))
            return ResponseType.TwoFactorCodeRequired;

        if (Code.Length != 6)
            return ResponseType.TwoFactorCodeInvalid;

        return ResponseType.Success;
    }
}

