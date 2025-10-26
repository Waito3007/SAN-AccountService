using AccountService.Application.Common.Models;
using AccountService.Application.Features.Users.Dtos;
using MediatR;

namespace AccountService.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// Query lấy thông tin chi tiết người dùng theo Id.
/// </summary>
public class GetUserByIdQuery : IRequest<ServiceResponse<UserDto>>
{
    public Guid UserId { get; set; }
}
