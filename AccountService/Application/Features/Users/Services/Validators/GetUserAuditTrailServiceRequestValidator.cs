using AccountService.Application.Common.Constants;
using AccountService.Application.Features.Users.Services.Requests;
using FluentValidation;

namespace AccountService.Application.Features.Users.Services.Validators;

/// <summary>
/// Validator cho GetUserAuditTrailServiceRequest.
/// </summary>
public class GetUserAuditTrailServiceRequestValidator : AbstractValidator<GetUserAuditTrailServiceRequest>
{
    public GetUserAuditTrailServiceRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(ErrorMessages.InvalidGuidFormat);

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber phải lớn hơn 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize phải nằm trong khoảng 1-100.");
    }
}
