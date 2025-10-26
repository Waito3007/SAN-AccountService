using AccountService.Application.Common.Constants;
using FluentValidation;

namespace AccountService.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Validator cho CreateUserCommand.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MinimumLength(3).WithMessage("Username phải có ít nhất 3 ký tự.")
            .MaximumLength(50).WithMessage("Username không được vượt quá 50 ký tự.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .EmailAddress().WithMessage(ErrorMessages.InvalidEmail)
            .MaximumLength(255).WithMessage("Email không được vượt quá 255 ký tự.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessages.RequiredField)
            .MinimumLength(8).WithMessage(ErrorMessages.PasswordTooShort);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleForEach(x => x.RoleIds)
            .NotEmpty().WithMessage(ErrorMessages.InvalidGuidFormat);

        When(x => x.Profile != null, () =>
        {
            RuleFor(x => x.Profile!.DisplayName)
                .MaximumLength(100).WithMessage("DisplayName không được vượt quá 100 ký tự.");
            RuleFor(x => x.Profile!.FirstName)
                .MaximumLength(100).WithMessage("FirstName không được vượt quá 100 ký tự.");
            RuleFor(x => x.Profile!.LastName)
                .MaximumLength(100).WithMessage("LastName không được vượt quá 100 ký tự.");
        });
    }
}
