using MediatR;
using AccountService.Application.Common.Models;

namespace AccountService.Application.Features.Users.Commands.SoftDeleteUser;

/// <summary>
/// Command để soft delete user
/// </summary>
public class SoftDeleteUserCommand : IRequest<ServiceResponse>
{
    public Guid UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
}