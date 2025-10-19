namespace AccountService.Application.Common.Constants;

/// <summary>
/// Tập trung tất cả các thông báo lỗi trong hệ thống
/// </summary>
public static class ErrorMessages
{
    #region Transaction Errors
    public const string TransactionAlreadyStarted = "Transaction đã được bắt đầu rồi.";
    public const string NoActiveTransaction = "Không có transaction nào đang chạy.";
    public const string TransactionCommitFailed = "Không thể commit transaction.";
    public const string TransactionRollbackFailed = "Không thể rollback transaction.";
    #endregion

    #region User Errors
    public const string UserNotFound = "Người dùng không tồn tại.";
    public const string UserAlreadyExists = "Người dùng đã tồn tại.";
    public const string EmailAlreadyExists = "Email đã được sử dụng.";
    public const string UsernameAlreadyExists = "Username đã được sử dụng.";
    public const string UserIsDeleted = "Người dùng đã bị xóa.";
    public const string UserIsLocked = "Tài khoản đã bị khóa.";
    public const string InvalidCredentials = "Email hoặc mật khẩu không đúng.";
    public const string EmailNotVerified = "Email chưa được xác thực.";
    #endregion

    #region Role Errors
    public const string RoleNotFound = "Vai trò không tồn tại.";
    public const string RoleAlreadyExists = "Vai trò đã tồn tại.";
    public const string RoleCodeAlreadyExists = "Mã vai trò đã được sử dụng.";
    public const string CannotDeleteSystemRole = "Không thể xóa vai trò hệ thống.";
    #endregion

    #region Permission Errors
    public const string PermissionNotFound = "Quyền không tồn tại.";
    public const string PermissionDenied = "Bạn không có quyền thực hiện hành động này.";
    public const string InsufficientPermissions = "Không đủ quyền để thực hiện thao tác này.";
    #endregion

    #region Validation Errors
    public const string InvalidEmail = "Email không hợp lệ.";
    public const string InvalidPhoneNumber = "Số điện thoại không hợp lệ.";
    public const string InvalidPassword = "Mật khẩu không đáp ứng yêu cầu.";
    public const string PasswordTooShort = "Mật khẩu phải có ít nhất 8 ký tự.";
    public const string PasswordTooWeak = "Mật khẩu phải chứa chữ hoa, chữ thường, số và ký tự đặc biệt.";
    public const string RequiredField = "Trường này là bắt buộc.";
    public const string InvalidGuidFormat = "Định dạng GUID không hợp lệ.";
    #endregion

    #region Token Errors
    public const string InvalidToken = "Token không hợp lệ.";
    public const string ExpiredToken = "Token đã hết hạn.";
    public const string RevokedToken = "Token đã bị thu hồi.";
    public const string TokenNotFound = "Token không tồn tại.";
    #endregion

    #region General Errors
    public const string OperationFailed = "Thao tác thất bại.";
    public const string UnexpectedError = "Đã xảy ra lỗi không mong muốn.";
    public const string DatabaseError = "Lỗi khi truy cập cơ sở dữ liệu.";
    public const string ConcurrencyError = "Dữ liệu đã được thay đổi bởi người khác.";
    #endregion

    #region Soft Delete Errors
    public const string SoftDeleteReasonRequired = "Lý do xóa là bắt buộc.";
    public const string CannotRestoreNonDeletedUser = "Không thể khôi phục người dùng chưa bị xóa.";
    public const string AlreadyDeleted = "Đã bị xóa trước đó.";
    #endregion

    #region Audit Errors
    public const string AuditLogNotFound = "Không tìm thấy lịch sử thao tác.";
    #endregion

    /// <summary>
    /// Format message với tham số
    /// </summary>
    public static string Format(string template, params object[] args)
    {
        return string.Format(template, args);
    }

    /// <summary>
    /// Tạo message với Id
    /// </summary>
    public static string NotFoundWithId(string entity, Guid id)
    {
        return $"{entity} với Id '{id}' không tồn tại.";
    }

    /// <summary>
    /// Tạo message với tên
    /// </summary>
    public static string NotFoundWithName(string entity, string name)
    {
        return $"{entity} '{name}' không tồn tại.";
    }

    /// <summary>
    /// Tạo message đã tồn tại
    /// </summary>
    public static string AlreadyExistsWith(string entity, string field, string value)
    {
        return $"{entity} với {field} '{value}' đã tồn tại.";
    }
}

