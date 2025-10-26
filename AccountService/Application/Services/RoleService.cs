using System.Linq.Expressions;
using AccountService.Application.Common.Constants;
using AccountService.Application.Common.Interfaces;
using AccountService.Application.Common.Models;
using AccountService.Application.Services.Interfaces;
using AccountService.Domain.Entities;
using AccountService.Domain.Type.Role;

namespace AccountService.Application.Services;

/// <summary>
/// Implementation của Role Service
/// </summary>
public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoleService> _logger;

    public RoleService(IUnitOfWork unitOfWork, ILogger<RoleService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateRoleResponse>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var roleRepository = _unitOfWork.Repository<Role>();

            // Check tồn tại
            if (await roleRepository.AnyAsync(r => r.Code == request.Code, cancellationToken))
            {
                _logger.LogWarning("Attempt to create role with existing code: {Code}", request.Code);
                return Result<CreateRoleResponse>.Failure(ErrorMessages.RoleCodeAlreadyExists);
            }

            // Tạo role
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            await roleRepository.AddAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role created successfully: {RoleId}, {Code}", role.Id, role.Code);
            var response = new CreateRoleResponse
            {
                Name = role.Name,
                Description = role.Description
            };
            return Result<CreateRoleResponse>.Success(response, SuccessMessages.RoleCreated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role: {Code}", request.Code);
            return Result<CreateRoleResponse>.Failure(ErrorMessages.OperationFailed);
        }
    }
    
    public async Task<Result<UpdateRoleResponse>> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var roleRepository = _unitOfWork.Repository<Role>();
            var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", roleId);
                return Result<UpdateRoleResponse>.Failure(ErrorMessages.RoleNotFound);
            }

            // Update role fields
            role.Name = request.Name;
            role.Description = request.Description;
            role.CreatedAt = DateTime.UtcNow;

            roleRepository.Update(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role updated successfully: {RoleId}", roleId);
            var response = new UpdateRoleResponse
            {
                Name = role.Name,
                Description = role.Description
            };
            return Result<UpdateRoleResponse>.Success(response, SuccessMessages.RoleUpdated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role: {RoleId}", roleId);
            return Result<UpdateRoleResponse>.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var roleRepository = _unitOfWork.Repository<Role>();
            var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", roleId);
                return Result.Failure(ErrorMessages.RoleNotFound);
            }

            roleRepository.Remove(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role deleted successfully: {RoleId}", roleId);
            return Result.Success(SuccessMessages.RoleDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role: {RoleId}", roleId);
            return Result.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<GetRoleResponse>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var roleRepository = _unitOfWork.Repository<Role>();
            var rolePermissionRepository = _unitOfWork.Repository<RolePermission>();
            var permissionRepository = _unitOfWork.Repository<Permission>();

            var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", roleId);
                return Result<GetRoleResponse>.Failure(ErrorMessages.RoleNotFound);
            }

            var rolePermissions = await rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId, cancellationToken);
            var permissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();

            var permissions = await permissionRepository.FindAsync(p => permissionIds.Contains(p.Id), cancellationToken);

            var response = new GetRoleResponse();

            return Result<GetRoleResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role: {RoleId}", roleId);
            return Result<GetRoleResponse>.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<GetRoleResponse>> GetRoleByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var roleRepository = _unitOfWork.Repository<Role>();
            var role = await roleRepository.FirstOrDefaultAsync(r => r.Code == code, cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("Role not found: {Code}", code);
                return Result<GetRoleResponse>.Failure(ErrorMessages.RoleNotFound);
            }

            var response = new GetRoleResponse
            {
                Name = role.Name,
                Description = role.Description,
                Permissions = null
            };

            return Result<GetRoleResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role: {Code}", code);
            return Result<GetRoleResponse>.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<List<GetRoleResponse>>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var roleRepository = _unitOfWork.Repository<Role>();
            var roles = await roleRepository.GetAllAsync(cancellationToken);

            var response = roles.Select(role => new GetRoleResponse
            {
                Name = role.Name,
                Description = role.Description,
                Permissions = null
            }).ToList();

            return Result<List<GetRoleResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all roles");
            return Result<List<GetRoleResponse>>.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> AssignRoleToUserAsync(AssignRoleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate
            var validationResults = request.Validate();
            if (validationResults.Any())
            {
                _logger.LogWarning("Validation failed for AssignRoleToUser");
                return Result.Failure(ErrorMessages.InvalidData);
            }

            var userRepository = _unitOfWork.Repository<User>();
            var roleRepository = _unitOfWork.Repository<Role>();
            var userRoleRepository = _unitOfWork.Repository<UserRole>();

            var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId);
                return Result.Failure(ErrorMessages.UserNotFound);
            }

            var role = await roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", request.RoleId);
                return Result.Failure(ErrorMessages.RoleNotFound);
            }

            // Check if already assigned
            if (await userRoleRepository.AnyAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId, cancellationToken))
            {
                _logger.LogWarning("Role already assigned to user: {UserId}, {RoleId}", request.UserId, request.RoleId);
                return Result.Failure("Role already assigned to user");
            }

            var userRole = new UserRole
            {
                UserId = request.UserId,
                RoleId = request.RoleId,
                AssignedAt = DateTime.UtcNow
            };

            await userRoleRepository.AddAsync(userRole, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role assigned to user successfully: {UserId}, {RoleId}", request.UserId, request.RoleId);
            return Result.Success("Role assigned successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to user: {UserId}, {RoleId}", request.UserId, request.RoleId);
            return Result.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> RemoveRoleFromUserAsync(RemoveRoleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate
            var validationResults = request.Validate();
            if (validationResults.Any())
            {
                _logger.LogWarning("Validation failed for RemoveRoleFromUser");
                return Result.Failure(ErrorMessages.InvalidData);
            }

            var userRoleRepository = _unitOfWork.Repository<UserRole>();
            var userRole = await userRoleRepository.FirstOrDefaultAsync(
                ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId, cancellationToken);

            if (userRole == null)
            {
                _logger.LogWarning("User role not found: {UserId}, {RoleId}", request.UserId, request.RoleId);
                return Result.Failure("User role not found");
            }

            userRoleRepository.Remove(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role removed from user successfully: {UserId}, {RoleId}", request.UserId, request.RoleId);
            return Result.Success("Role removed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role from user: {UserId}, {RoleId}", request.UserId, request.RoleId);
            return Result.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<List<GetRoleResponse>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRoleRepository = _unitOfWork.Repository<UserRole>();
            var roleRepository = _unitOfWork.Repository<Role>();

            var userRoles = await userRoleRepository.FindAsync(ur => ur.UserId == userId, cancellationToken);
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

            var roles = await roleRepository.FindAsync(r => roleIds.Contains(r.Id), cancellationToken);

            var response = roles.Select(role => new GetRoleResponse
            {
                Name = role.Name,
                Description = role.Description,
                Permissions = null
            }).ToList();

            return Result<List<GetRoleResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user roles: {UserId}", userId);
            return Result<List<GetRoleResponse>>.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result> AssignPermissionsToRoleAsync(AssignPermissionsRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate request
            if (request.RoleId == Guid.Empty || !request.PermissionCodes.Any())
            {
                return Result.Failure("Invalid role ID or permission codes.");
            }

            var role = await _unitOfWork.Repository<Role>().GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
            {
                return Result.Failure("Role not found.");
            }

            foreach (var permissionCode in request.PermissionCodes)
            {
                if (!int.TryParse(permissionCode, out var permissionCodeInt))
                {
                    _logger.LogWarning("Invalid permission code format: {PermissionCode}", permissionCode);
                    continue;
                }

                var permission = await _unitOfWork.Repository<Permission>().FirstOrDefaultAsync(
                    p => p.Code == permissionCodeInt, cancellationToken);

                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionCode}", permissionCode);
                    continue;
                }

                var rolePermission = new RolePermission
                {
                    RoleId = request.RoleId,
                    PermissionId = permission.Id
                };

                await _unitOfWork.Repository<RolePermission>().AddAsync(rolePermission, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success("Permissions assigned successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permissions to role: {RoleId}", request.RoleId);
            return Result.Failure("An error occurred while assigning permissions.");
        }
    }

    public async Task<Result> RemovePermissionsFromRoleAsync(RemovePermissionsRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate
            var validationResults = request.Validate();
            if (validationResults.Any())
            {
                _logger.LogWarning("Validation failed for RemovePermissionsFromRole");
                return Result.Failure(ErrorMessages.InvalidData);
            }

            var permissionRepository = _unitOfWork.Repository<Permission>();
            var rolePermissionRepository = _unitOfWork.Repository<RolePermission>();

            foreach (var permissionCode in request.PermissionCodes)
            {
                if (!int.TryParse(permissionCode, out var permissionCodeInt))
                {
                    _logger.LogWarning("Invalid permission code format: {PermissionCode}", permissionCode);
                    continue;
                }

                var permission = await permissionRepository.FirstOrDefaultAsync(p => p.Code == permissionCodeInt, cancellationToken);
                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionCode}", permissionCode);
                    continue;
                }

                var rolePermission = await rolePermissionRepository.FirstOrDefaultAsync(
                    rp => rp.RoleId == request.RoleId && rp.PermissionId == permission.Id, cancellationToken);

                if (rolePermission != null)
                {
                    rolePermissionRepository.Remove(rolePermission);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Permissions removed from role successfully: {RoleId}", request.RoleId);
            return Result.Success("Permissions removed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing permissions from role: {RoleId}", request.RoleId);
            return Result.Failure(ErrorMessages.OperationFailed);
        }
    }

    public async Task<Result<GetRoleResponse>> GetRoleWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var roleRepository = _unitOfWork.Repository<Role>();
            var rolePermissionRepository = _unitOfWork.Repository<RolePermission>();
            var permissionRepository = _unitOfWork.Repository<Permission>();

            var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
            {
                _logger.LogWarning("Role not found: {RoleId}", roleId);
                return Result<GetRoleResponse>.Failure(ErrorMessages.RoleNotFound);
            }

            var rolePermissions = await rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId, cancellationToken);
            var permissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();

            var permissions = await permissionRepository.FindAsync(p => permissionIds.Contains(p.Id), cancellationToken);

            var response = new GetRoleResponse
            {
                Name = role.Name,
                Description = role.Description,
                Permissions = permissions.Select(p => new Permission
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    Resource = p.Resource,
                    Action = p.Action
                }).ToList()
            };

            return Result<GetRoleResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role with permissions: {RoleId}", roleId);
            return Result<GetRoleResponse>.Failure(ErrorMessages.OperationFailed);
        }
    }
}