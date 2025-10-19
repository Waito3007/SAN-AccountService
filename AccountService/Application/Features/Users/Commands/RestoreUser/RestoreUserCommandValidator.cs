using FluentValidation;

namespace AccountService.Application.Features.Users.Commands.RestoreUser;

/// <summary>
/// Validator cho RestoreUserCommand
/// </summary>
public class RestoreUserCommandValidator : AbstractValidator<RestoreUserCommand>
{
    public RestoreUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId là bắt buộc.");
    }
}

