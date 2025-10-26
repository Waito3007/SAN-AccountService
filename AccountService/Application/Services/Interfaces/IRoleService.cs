using AccountService.Application.Common.Models;
using AccountService.Domain.Type.Role;

namespace AccountService.Application.Services.Interfaces;

/// <summary>
/// Interface cho Role Service
/// </summary>
public interface IRoleService
{
    // CRUD
    Task<Result<CreateRoleResponse>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task<Result<UpdateRoleResponse>> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<GetRoleResponse>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<GetRoleResponse>> GetRoleByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<GetRoleResponse>>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    
    // Role-User Management
    Task<Result> AssignRoleToUserAsync(AssignRoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveRoleFromUserAsync(RemoveRoleRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<GetRoleResponse>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    
    // Role-Permission Management
    Task<Result> AssignPermissionsToRoleAsync(AssignPermissionsRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemovePermissionsFromRoleAsync(RemovePermissionsRequest request, CancellationToken cancellationToken = default);
    Task<Result<GetRoleResponse>> GetRoleWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
}