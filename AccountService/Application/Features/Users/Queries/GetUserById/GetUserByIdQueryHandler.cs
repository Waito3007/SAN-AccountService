using AccountService.Application.Common.Constants;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Application.Features.Users.Dtos;
using AccountService.Application.Features.Users.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// Handler cho GetUserByIdQuery.
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ServiceResponse<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetUserByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ServiceResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetUserDetailAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found when querying detail.", request.UserId);
            return ServiceResponse<UserDto>.Failure(ErrorMessages.UserNotFound);
        }

        return ServiceResponse<UserDto>.Success(user.ToDto());
    }
}
