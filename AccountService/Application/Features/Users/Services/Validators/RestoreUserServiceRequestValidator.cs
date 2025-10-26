using AccountService.Application.Common.Constants;
using AccountService.Application.Features.Users.Services.Requests;
using FluentValidation;

namespace AccountService.Application.Features.Users.Services.Validators;

/// <summary>
/// Validator cho RestoreUserServiceRequest.
/// </summary>
public class RestoreUserServiceRequestValidator : AbstractValidator<RestoreUserServiceRequest>
{
    public RestoreUserServiceRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(ErrorMessages.InvalidGuidFormat);
    }
}
