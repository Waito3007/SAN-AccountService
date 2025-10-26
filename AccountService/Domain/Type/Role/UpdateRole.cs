using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.Role;

public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public List<ValidationResult> Validate()
    {
        var results = new List<ValidationResult>();
        if (string.IsNullOrWhiteSpace(Name))
            results.Add(new ValidationResult { Type = ResponseType.NameRequired });
        return results;
    }
}

public class UpdateRoleResponse
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}