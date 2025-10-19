using System.Security.Claims;
using AccountService.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using AccountService.Application.Common.Interfaces;
using AccountService.Domain.Common;
namespace AccountService.Infrastructure.Services
{
    /// <summary>
    /// Service để lấy thông tin user hiện tại từ HttpContext
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
            }
        }

        public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public bool HasPermission(int permissionCode)
        {
            var permsClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("perms")?.Value;
            if (string.IsNullOrEmpty(permsClaim)) return false;

            var permissions = permsClaim.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => int.TryParse(p.Trim(), out var code) ? code : -1)
                .ToHashSet();

            return permissions.Contains(permissionCode);
        }
    }

}



