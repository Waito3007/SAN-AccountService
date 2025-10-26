using AccountService.Domain.Enums;

namespace AccountService.Domain.Type.User;

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public AccountType AccountType { get; set; }

    public ResponseType Validate()
    {
        if (string.IsNullOrWhiteSpace(Username))
            return ResponseType.UsernameRequired;

        if (string.IsNullOrWhiteSpace(Email))
            return ResponseType.EmailRequired;

        if (string.IsNullOrWhiteSpace(Password))
            return ResponseType.PasswordRequired;

        if (Password.Length < 6)
            return ResponseType.PasswordTooShort;

        if (string.IsNullOrWhiteSpace(FirstName))
            return ResponseType.FirstNameRequired;

        if (string.IsNullOrWhiteSpace(LastName))
            return ResponseType.LastNameRequired;

        if (!IsValidEmail(Email))
            return ResponseType.EmailInvalid;

        return ResponseType.Success;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

public class CreateUserResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

