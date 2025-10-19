using Microsoft.AspNetCore.Mvc;
using MediatR;
using AccountService.Authorization;
using AccountService.Domain.Enums;
using AccountService.Application.Features.Users.Commands.SoftDeleteUser;
using AccountService.Application.Features.Users.Commands.RestoreUser;
using AccountService.Application.Features.Users.Queries.GetUserAuditTrail;

namespace AccountService.Controllers.v1;

/// <summary>
/// Controller quản lý Users
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Soft delete user
    /// </summary>
    [HttpPost("{id}/soft-delete")]
    [HasPermission(PermissionCode.SoftDeleteUser)]
    public async Task<IActionResult> SoftDeleteUser(Guid id, [FromBody] SoftDeleteUserRequest request)
    {
        var command = new SoftDeleteUserCommand
        {
            UserId = id,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Khôi phục user đã bị soft delete
    /// </summary>
    [HttpPost("{id}/restore")]
    [HasPermission(PermissionCode.RestoreUser)]
    public async Task<IActionResult> RestoreUser(Guid id)
    {
        var command = new RestoreUserCommand { UserId = id };
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy audit trail của user
    /// </summary>
    [HttpGet("{id}/audit")]
    [HasPermission(PermissionCode.ReadAuditLog)]
    public async Task<IActionResult> GetUserAuditTrail(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetUserAuditTrailQuery
        {
            UserId = id,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}

/// <summary>
/// Request DTO cho soft delete
/// </summary>
public class SoftDeleteUserRequest
{
    public string Reason { get; set; } = string.Empty;
}

