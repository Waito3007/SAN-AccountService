using AccountService.Domain.Enums;
using AccountService.Domain.Type.Role;

namespace AccountService.Domain.Type.Role;

public class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public List<ValidationResult> Validate()
    {
        var results = new List<ValidationResult>();
        if (UserId == Guid.Empty)
            results.Add(new ValidationResult { Type = ResponseType.UserIdInvalid });
        if (RoleId == Guid.Empty)
            results.Add(new ValidationResult { Type = ResponseType.RoleIdInvalid });
        return results;
    }
}

public class RemoveRoleRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public List<ValidationResult> Validate()
    {
        var results = new List<ValidationResult>();
        if (UserId == Guid.Empty)
            results.Add(new ValidationResult { Type = ResponseType.UserIdInvalid });
        if (RoleId == Guid.Empty)
            results.Add(new ValidationResult { Type = ResponseType.RoleIdInvalid });
        return results;
    }
}

