using AccountService.Application.Common.Constants;
using AccountService.Application.Features.Users.Services.Requests;
using FluentValidation;

namespace AccountService.Application.Features.Users.Services.Validators;

/// <summary>
/// Validator cho GetUserByIdServiceRequest.
/// </summary>
public class GetUserByIdServiceRequestValidator : AbstractValidator<GetUserByIdServiceRequest>
{
    public GetUserByIdServiceRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(ErrorMessages.InvalidGuidFormat);
    }
}
