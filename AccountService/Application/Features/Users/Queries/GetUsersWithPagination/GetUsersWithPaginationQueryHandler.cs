using System;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Application.Features.Users.Dtos;
using AccountService.Application.Features.Users.Mappings;
using MediatR;

namespace AccountService.Application.Features.Users.Queries.GetUsersWithPagination;

/// <summary>
/// Handler trả về danh sách người dùng phân trang.
/// </summary>
public class GetUsersWithPaginationQueryHandler : IRequestHandler<GetUsersWithPaginationQuery, ServiceResponse<PaginatedList<UserDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUsersWithPaginationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<PaginatedList<UserDto>>> Handle(GetUsersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);

        var users = await _unitOfWork.Users.GetUsersWithPaginationAsync(pageNumber, pageSize, cancellationToken);
        var dto = users.ToDto();

        return ServiceResponse<PaginatedList<UserDto>>.Success(dto);
    }
}
