namespace AccountService.Domain.Enums;

/// <summary>
/// Nhà cung cấp xác thực
/// </summary>
public enum AuthProvider
{
    Local = 0,             // Đăng ký thông thường
    Google = 1,
    Facebook = 2,
    Apple = 3,
    Microsoft = 4
}

