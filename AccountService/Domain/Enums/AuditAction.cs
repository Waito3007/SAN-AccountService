namespace AccountService.Domain.Enums;

/// <summary>
/// Hành động Audit Log
/// </summary>
public enum AuditAction
{
    Create = 0,
    Read = 1,
    Update = 2,
    SoftDelete = 3,
    Restore = 4,
    Login = 5,
    Logout = 6,
    PasswordChange = 7,
    PasswordReset = 8,
    EmailVerification = 9,
    PhoneVerification = 10,
    TwoFactorEnabled = 11,
    TwoFactorDisabled = 12,
    AccountLocked = 13,
    AccountUnlocked = 14,
    AccountActivated = 15,
    AccountDeactivated = 16
}