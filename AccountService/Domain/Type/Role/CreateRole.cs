using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.Role;

public class ValidationResult
{
    public ResponseType Type { get; set; }
}

public class CreateRoleRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public List<ValidationResult> Validate()
    {
        var results = new List<ValidationResult>();
        if (string.IsNullOrWhiteSpace(Code))
            results.Add(new ValidationResult { Type = ResponseType.CodeRequired });
        if (string.IsNullOrWhiteSpace(Name))
            results.Add(new ValidationResult { Type = ResponseType.NameRequired });
        return results;
    }
}

public class CreateRoleResponse
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}