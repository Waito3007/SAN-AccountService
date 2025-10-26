using System.Threading;
using System.Threading.Tasks;
using AccountService.Application.Common.Models;
using AccountService.Application.Features.Users.Dtos;
using AccountService.Application.Features.Users.Services.Requests;

namespace AccountService.Application.Features.Users.Services;

/// <summary>
/// Service interface xử lý nghiệp vụ người dùng theo request-response pattern.
/// </summary>
public interface IUserService
{
    Task<Result<UserDto>> CreateAsync(CreateUserServiceRequest request, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> UpdateAsync(UpdateUserServiceRequest request, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> GetByIdAsync(GetUserByIdServiceRequest request, CancellationToken cancellationToken = default);

    Task<Result<PaginatedList<UserDto>>> GetListAsync(GetUsersServiceRequest request, CancellationToken cancellationToken = default);

    Task<Result> SoftDeleteAsync(SoftDeleteUserServiceRequest request, CancellationToken cancellationToken = default);

    Task<Result> RestoreAsync(RestoreUserServiceRequest request, CancellationToken cancellationToken = default);

    Task<Result<PaginatedList<AuditLogDto>>> GetAuditTrailAsync(GetUserAuditTrailServiceRequest request, CancellationToken cancellationToken = default);
}
