using AccountService.Application.Features.Users.Services;
using AccountService.Application.Features.Users.Services.Requests;
using AccountService.Authorization;
using AccountService.Contracts.Users;
using AccountService.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers.v1;

/// <summary>
/// Controller quản lý Users.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Tạo mới người dùng.
    /// </summary>
    [HttpPost]
    [HasPermission(PermissionCode.CreateUser)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var cancellationToken = HttpContext.RequestAborted;

        var serviceRequest = new CreateUserServiceRequest
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber,
            AccountType = request.AccountType,
            Status = request.Status,
            RoleIds = request.RoleIds,
            Profile = request.Profile == null ? null : new CreateUserProfileServiceRequest
            {
                FirstName = request.Profile.FirstName,
                LastName = request.Profile.LastName,
                MiddleName = request.Profile.MiddleName,
                DisplayName = request.Profile.DisplayName,
                DateOfBirth = request.Profile.DateOfBirth,
                Gender = request.Profile.Gender,
                Avatar = request.Profile.Avatar,
                Bio = request.Profile.Bio,
                AddressLine1 = request.Profile.AddressLine1,
                AddressLine2 = request.Profile.AddressLine2,
                City = request.Profile.City,
                State = request.Profile.State,
                Country = request.Profile.Country,
                PostalCode = request.Profile.PostalCode,
                Timezone = request.Profile.Timezone,
                Language = request.Profile.Language
            }
        };

        var result = await _userService.CreateAsync(serviceRequest, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Create user failed for {Email}: {Errors}", request.Email, string.Join(",", result.Errors));
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetUserById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Cập nhật thông tin người dùng.
    /// </summary>
    [HttpPut("{id}")]
    [HasPermission(PermissionCode.UpdateUser)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var cancellationToken = HttpContext.RequestAborted;

        var serviceRequest = new UpdateUserServiceRequest
        {
            UserId = id,
            Username = request.Username,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Status = request.Status,
            AccountType = request.AccountType,
            EmailVerified = request.EmailVerified,
            PhoneVerified = request.PhoneVerified,
            TwoFactorEnabled = request.TwoFactorEnabled,
            RoleIds = request.RoleIds,
            Profile = request.Profile == null ? null : new UpdateUserProfileServiceRequest
            {
                FirstName = request.Profile.FirstName,
                LastName = request.Profile.LastName,
                MiddleName = request.Profile.MiddleName,
                DisplayName = request.Profile.DisplayName,
                DateOfBirth = request.Profile.DateOfBirth,
                Gender = request.Profile.Gender,
                Avatar = request.Profile.Avatar,
                Bio = request.Profile.Bio,
                AddressLine1 = request.Profile.AddressLine1,
                AddressLine2 = request.Profile.AddressLine2,
                City = request.Profile.City,
                State = request.Profile.State,
                Country = request.Profile.Country,
                PostalCode = request.Profile.PostalCode,
                Timezone = request.Profile.Timezone,
                Language = request.Profile.Language
            }
        };

        var result = await _userService.UpdateAsync(serviceRequest, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Update user failed for {UserId}: {Errors}", id, string.Join(",", result.Errors));
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin người dùng theo Id.
    /// </summary>
    [HttpGet("{id}")]
    [HasPermission(PermissionCode.ReadUser)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var serviceRequest = new GetUserByIdServiceRequest { UserId = id };
        var result = await _userService.GetByIdAsync(serviceRequest, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Get user by id failed for {UserId}: {Errors}", id, string.Join(",", result.Errors));
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách người dùng phân trang.
    /// </summary>
    [HttpGet]
    [HasPermission(PermissionCode.ReadUser)]
    public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var serviceRequest = new GetUsersServiceRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _userService.GetListAsync(serviceRequest, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Get users failed: {Errors}", string.Join(",", result.Errors));
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Soft delete user.
    /// </summary>
    [HttpPost("{id}/soft-delete")]
    [HasPermission(PermissionCode.SoftDeleteUser)]
    public async Task<IActionResult> SoftDeleteUser(Guid id, [FromBody] SoftDeleteUserRequest request)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var serviceRequest = new SoftDeleteUserServiceRequest
        {
            UserId = id,
            Reason = request.Reason
        };

        var result = await _userService.SoftDeleteAsync(serviceRequest, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Soft delete user failed for {UserId}: {Errors}", id, string.Join(",", result.Errors));
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Khôi phục user đã bị soft delete.
    /// </summary>
    [HttpPost("{id}/restore")]
    [HasPermission(PermissionCode.RestoreUser)]
    public async Task<IActionResult> RestoreUser(Guid id)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var serviceRequest = new RestoreUserServiceRequest { UserId = id };
        var result = await _userService.RestoreAsync(serviceRequest, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Restore user failed for {UserId}: {Errors}", id, string.Join(",", result.Errors));
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy audit trail của user.
    /// </summary>
    [HttpGet("{id}/audit")]
    [HasPermission(PermissionCode.ReadAuditLog)]
    public async Task<IActionResult> GetUserAuditTrail(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var serviceRequest = new GetUserAuditTrailServiceRequest
        {
            UserId = id,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _userService.GetAuditTrailAsync(serviceRequest, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Get user audit trail failed for {UserId}: {Errors}", id, string.Join(",", result.Errors));
            return BadRequest(result);
        }

        return Ok(result);
    }
}

