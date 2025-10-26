using AccountService.Application.Common.Constants;
using AccountService.Application.Features.Users.Services.Requests;
using FluentValidation;

namespace AccountService.Application.Features.Users.Services.Validators;

/// <summary>
/// Validator cho SoftDeleteUserServiceRequest.
/// </summary>
public class SoftDeleteUserServiceRequestValidator : AbstractValidator<SoftDeleteUserServiceRequest>
{
    public SoftDeleteUserServiceRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(ErrorMessages.InvalidGuidFormat);

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Lý do xóa là bắt buộc.")
            .MaximumLength(500).WithMessage("Lý do không được vượt quá 500 ký tự.");
    }
}
