using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.Role;

public class AssignPermissionsRequest
{
    public Guid RoleId { get; set; }
    public List<string> PermissionCodes { get; set; } = new();

    public List<ValidationResult> Validate()
    {
        var results = new List<ValidationResult>();
        if (RoleId == Guid.Empty)
            results.Add(new ValidationResult { Type = ResponseType.RoleIdInvalid });
        if (PermissionCodes.Count == 0)
            results.Add(new ValidationResult { Type = ResponseType.PermissionCodesInvalid });
        return results;
    }
}

public class RemovePermissionsRequest
{
    public Guid RoleId { get; set; }
    public List<string> PermissionCodes { get; set; } = new();

    public List<ValidationResult> Validate()
    {
        var results = new List<ValidationResult>();
        if (RoleId == Guid.Empty)
            results.Add(new ValidationResult { Type = ResponseType.RoleIdInvalid });
        if (PermissionCodes.Count == 0)
            results.Add(new ValidationResult { Type = ResponseType.PermissionCodesInvalid });
        return results;
    }
}


