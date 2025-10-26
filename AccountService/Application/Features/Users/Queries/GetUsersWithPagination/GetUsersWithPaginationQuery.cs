using AccountService.Application.Common.Models;
using AccountService.Application.Features.Users.Dtos;
using MediatR;

namespace AccountService.Application.Features.Users.Queries.GetUsersWithPagination;

/// <summary>
/// Query lấy danh sách người dùng phân trang.
/// </summary>
public class GetUsersWithPaginationQuery : IRequest<ServiceResponse<PaginatedList<UserDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
