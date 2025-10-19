namespace AccountService.Domain.Enums;

/// <summary>
/// Trạng thái của người dùng
/// </summary>
public enum UserStatus
{
    Pending = 0,           // Chờ xác thực email
    Active = 1,            // Đang hoạt động
    Inactive = 2,          // Không hoạt động
    Suspended = 3,         // Tạm khóa
    Locked = 4,            // Bị khóa
    Banned = 5             // Bị cấm
}