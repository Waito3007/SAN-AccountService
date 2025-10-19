namespace AccountService.Domain.Enums;

/// <summary>
/// Loại tài khoản
/// </summary>
public enum AccountType
{
    Customer = 0,          // Khách hàng
    Seller = 1,            // Người bán
    Admin = 2,             // Quản trị viên
    SuperAdmin = 3,        // Quản trị viên cấp cao
    Employee = 4           // Nhân viên
}

