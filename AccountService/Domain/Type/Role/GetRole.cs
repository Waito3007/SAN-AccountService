using AccountService.Domain.Entities;

namespace AccountService.Domain.Type.Role;

public class GetRoleResponse
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Permission>? Permissions { get; set; }
}

