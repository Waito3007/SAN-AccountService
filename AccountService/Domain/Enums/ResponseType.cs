using System.ComponentModel;

namespace AccountService.Domain.Enums;

public enum ResponseType
{
    [Description("Thành công")]
    Success = 0,
    [Description("Có lỗi trong quá trình xử lý")]
    Exception = -1,
    [Description("Dữ liệu không đúng quy định")]
    InvalidData = -2,
    [Description("Code không được để trống")]
    CodeRequired = -10,
    [Description("Name không được để trống")]
    NameRequired = -11,
    [Description("Role ID không hợp lệ")]
    RoleIdInvalid = -12,
    [Description("User ID không hợp lệ")]
    UserIdInvalid = -13,
    [Description("Permission codes không hợp lệ")]
    PermissionCodesInvalid = -14,
    
    // User validation
    [Description("Username không được để trống")]
    UsernameRequired = -20,
    [Description("Email không được để trống")]
    EmailRequired = -21,
    [Description("Password không được để trống")]
    PasswordRequired = -22,
    [Description("Password phải có ít nhất 6 ký tự")]
    PasswordTooShort = -23,
    [Description("FirstName không được để trống")]
    FirstNameRequired = -24,
    [Description("LastName không được để trống")]
    LastNameRequired = -25,
    [Description("Email không hợp lệ")]
    EmailInvalid = -26,
    [Description("Không có dữ liệu để cập nhật")]
    NoDataToUpdate = -27,
    [Description("Current password không được để trống")]
    CurrentPasswordRequired = -28,
    [Description("New password không được để trống")]
    NewPasswordRequired = -29,
    [Description("Password xác nhận không khớp")]
    PasswordNotMatch = -30,
    [Description("Mật khẩu mới không được trùng mật khẩu cũ")]
    NewPasswordSameAsOld = -31,
    
    // Auth validation
    [Description("Email hoặc Username không được để trống")]
    EmailOrUsernameRequired = -40,
    [Description("Refresh token không được để trống")]
    RefreshTokenRequired = -41,
    [Description("Token không được để trống")]
    TokenRequired = -42,
    [Description("Mã 2FA không được để trống")]
    TwoFactorCodeRequired = -43,
    [Description("Mã 2FA không hợp lệ")]
    TwoFactorCodeInvalid = -44,
    [Description("Gửi lại email thất bại")]
    OperationFailed = -50,
   
}