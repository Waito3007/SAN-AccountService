namespace AccountService.Application.Common.Interfaces;

/// <summary>
/// Interface cho dịch vụ lấy thông tin user hiện tại
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    bool HasPermission(int permissionCode);
}

