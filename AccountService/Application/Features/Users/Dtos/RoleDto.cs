namespace AccountService.Application.Features.Users.Dtos;

/// <summary>
/// Dto mô tả thông tin Role gán cho người dùng.
/// </summary>
public class RoleDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
