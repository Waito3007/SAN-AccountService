using MediatR;
using AccountService.Application.Common.Models;

namespace AccountService.Application.Features.Users.Commands.RestoreUser;

/// <summary>
/// Command để khôi phục user đã bị soft delete
/// </summary>
public class RestoreUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
}

