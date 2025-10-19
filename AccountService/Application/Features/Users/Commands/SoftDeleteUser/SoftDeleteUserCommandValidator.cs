using FluentValidation;

namespace AccountService.Application.Features.Users.Commands.SoftDeleteUser;

/// <summary>
/// Validator cho SoftDeleteUserCommand
/// </summary>
public class SoftDeleteUserCommandValidator : AbstractValidator<SoftDeleteUserCommand>
{
    public SoftDeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId là bắt buộc.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Lý do xóa là bắt buộc.")
            .MaximumLength(500).WithMessage("Lý do không được vượt quá 500 ký tự.");
    }
}

