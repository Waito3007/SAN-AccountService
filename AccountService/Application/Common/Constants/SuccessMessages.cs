namespace AccountService.Application.Common.Constants;

/// <summary>
/// Tập trung tất cả các thông báo thành công trong hệ thống
/// </summary>
public static class SuccessMessages
{
    #region User Messages
    public const string UserCreated = "Người dùng đã được tạo thành công.";
    public const string UserUpdated = "Thông tin người dùng đã được cập nhật.";
    public const string UserDeleted = "Người dùng đã được xóa thành công.";
    public const string UserRestored = "Người dùng đã được khôi phục.";
    public const string UserActivated = "Tài khoản đã được kích hoạt.";
    public const string UserDeactivated = "Tài khoản đã bị vô hiệu hóa.";
    public const string UserLocked = "Tài khoản đã bị khóa.";
    public const string UserUnlocked = "Tài khoản đã được mở khóa.";
    #endregion

    #region Auth Messages
    public const string LoginSuccessful = "Đăng nhập thành công.";
    public const string LogoutSuccessful = "Đăng xuất thành công.";
    public const string PasswordChanged = "Mật khẩu đã được thay đổi.";
    public const string PasswordReset = "Mật khẩu đã được đặt lại.";
    public const string EmailVerified = "Email đã được xác thực.";
    public const string TokenRefreshed = "Token đã được làm mới.";
    #endregion

    #region Role Messages
    public const string RoleCreated = "Vai trò đã được tạo thành công.";
    public const string RoleUpdated = "Vai trò đã được cập nhật.";
    public const string RoleDeleted = "Vai trò đã được xóa.";
    public const string RoleAssigned = "Vai trò đã được gán cho người dùng.";
    public const string RoleRemoved = "Vai trò đã được gỡ bỏ khỏi người dùng.";
    #endregion

    #region Permission Messages
    public const string PermissionAssigned = "Quyền đã được gán cho vai trò.";
    public const string PermissionRemoved = "Quyền đã được gỡ bỏ khỏi vai trò.";
    #endregion

    #region Profile Messages
    public const string ProfileUpdated = "Hồ sơ đã được cập nhật.";
    public const string AvatarUpdated = "Ảnh đại diện đã được cập nhật.";
    #endregion

    #region Transaction Messages
    public const string TransactionCommitted = "Transaction đã được commit thành công.";
    public const string TransactionRolledBack = "Transaction đã được rollback.";
    #endregion

    #region General Messages
    public const string OperationSuccessful = "Thao tác thành công.";
    public const string SavedSuccessfully = "Đã lưu thành công.";
    #endregion

    /// <summary>
    /// Format message với tham số
    /// </summary>
    public static string Format(string template, params object[] args)
    {
        return string.Format(template, args);
    }
}

