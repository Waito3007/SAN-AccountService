using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.User;

public class UpdateUserRequest
{
    public string? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Bio { get; set; }

    public ResponseType Validate()
    {
        // At least one field should be provided for update
        if (string.IsNullOrWhiteSpace(PhoneNumber) && 
            string.IsNullOrWhiteSpace(FirstName) && 
            string.IsNullOrWhiteSpace(LastName) &&
            string.IsNullOrWhiteSpace(MiddleName) &&
            !Gender.HasValue &&
            !DateOfBirth.HasValue &&
            string.IsNullOrWhiteSpace(Bio))
        {
            return ResponseType.NoDataToUpdate;
        }

        return ResponseType.Success;
    }
}

public class UpdateUserResponse
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
}

