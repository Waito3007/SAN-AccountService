using AccountService.Application.Common.Models;
using AccountService.Application.Services.Interfaces;
using AccountService.Authorization;
using AccountService.Domain.Enums;
using AccountService.Domain.Type.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IRoleService roleService, ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    /// <summary>
    /// Tạo role mới
    /// </summary>
    [HttpPost]
    [HasPermission(PermissionCode.RoleCreate)]
    public async Task<ActionResult<Result<CreateRoleResponse>>> CreateRole(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _roleService.CreateRoleAsync(request, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetRoleById), new { roleId = Guid.NewGuid() }, result);
    }

    /// <summary>
    /// Cập nhật role
    /// </summary>
    [HttpPut("{roleId:guid}")]
    [HasPermission(PermissionCode.RoleUpdate)]
    public async Task<ActionResult<Result<UpdateRoleResponse>>> UpdateRole(Guid roleId, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {

        var result = await _roleService.UpdateRoleAsync(roleId, request, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Xóa role
    /// </summary>
    [HttpDelete("{roleId:guid}")]
    [HasPermission(PermissionCode.RoleDelete)]
    public async Task<ActionResult<Result>> DeleteRole(Guid roleId, CancellationToken cancellationToken)
    {
        var result = await _roleService.DeleteRoleAsync(roleId, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Lấy role theo ID
    /// </summary>
    [HttpGet("{roleId:guid}")]
    [HasPermission(PermissionCode.RoleRead)]
    public async Task<ActionResult<Result<GetRoleResponse>>> GetRoleById(Guid roleId, CancellationToken cancellationToken)
    {
        var result = await _roleService.GetRoleByIdAsync(roleId, cancellationToken);
        
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Lấy role theo code
    /// </summary>
    [HttpGet("by-code/{code}")]
    [HasPermission(PermissionCode.RoleRead)]
    public async Task<ActionResult<Result<GetRoleResponse>>> GetRoleByCode(string code, CancellationToken cancellationToken)
    {
        var result = await _roleService.GetRoleByCodeAsync(code, cancellationToken);
        
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Lấy tất cả roles
    /// </summary>
    [HttpGet]
    [HasPermission(PermissionCode.RoleRead)]
    public async Task<ActionResult<Result<List<GetRoleResponse>>>> GetAllRoles(CancellationToken cancellationToken)
    {
        var result = await _roleService.GetAllRolesAsync(cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Lấy role với permissions
    /// </summary>
    [HttpGet("{roleId:guid}/permissions")]
    [HasPermission(PermissionCode.RoleRead)]
    public async Task<ActionResult<Result<GetRoleResponse>>> GetRoleWithPermissions(Guid roleId, CancellationToken cancellationToken)
    {
        var result = await _roleService.GetRoleWithPermissionsAsync(roleId, cancellationToken);
        
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Gán role cho user
    /// </summary>
    [HttpPost("assign")]
    [HasPermission(PermissionCode.RoleAssign)]
    public async Task<ActionResult<Result>> AssignRoleToUser([FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        var validationResults = request.Validate();
        if (validationResults.Any())
        {
            return BadRequest(Result.Failure("Validation failed"));
        }

        var result = await _roleService.AssignRoleToUserAsync(request, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Xóa role khỏi user
    /// </summary>
    [HttpPost("remove")]
    [HasPermission(PermissionCode.RoleAssign)]
    public async Task<ActionResult<Result>> RemoveRoleFromUser([FromBody] RemoveRoleRequest request, CancellationToken cancellationToken)
    {
        var validationResults = request.Validate();
        if (validationResults.Any())
        {
            return BadRequest(Result.Failure("Validation failed"));
        }

        var result = await _roleService.RemoveRoleFromUserAsync(request, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Lấy roles của user
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [HasPermission(PermissionCode.RoleRead)]
    public async Task<ActionResult<Result<List<GetRoleResponse>>>> GetUserRoles(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _roleService.GetUserRolesAsync(userId, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Gán permissions cho role
    /// </summary>
    [HttpPost("{roleId:guid}/permissions")]
    [HasPermission(PermissionCode.PermissionAssign)]
    public async Task<ActionResult<Result>> AssignPermissionsToRole(Guid roleId, [FromBody] List<string> permissionCodes, CancellationToken cancellationToken)
    {
        var request = new AssignPermissionsRequest
        {
            RoleId = roleId,
            PermissionCodes = permissionCodes
        };

        var validationResults = request.Validate();
        if (validationResults.Any())
        {
            return BadRequest(Result.Failure("Validation failed"));
        }

        var result = await _roleService.AssignPermissionsToRoleAsync(request, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Xóa permissions khỏi role
    /// </summary>
    [HttpDelete("{roleId:guid}/permissions")]
    [HasPermission(PermissionCode.PermissionAssign)]
    public async Task<ActionResult<Result>> RemovePermissionsFromRole(Guid roleId, [FromBody] List<string> permissionCodes, CancellationToken cancellationToken)
    {
        var request = new RemovePermissionsRequest
        {
            RoleId = roleId,
            PermissionCodes = permissionCodes
        };

        var validationResults = request.Validate();
        if (validationResults.Any())
        {
            return BadRequest(Result.Failure("Validation failed"));
        }

        var result = await _roleService.RemovePermissionsFromRoleAsync(request, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}

