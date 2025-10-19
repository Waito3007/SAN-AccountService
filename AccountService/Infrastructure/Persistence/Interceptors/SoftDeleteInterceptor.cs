using AccountService.Application.Common.Interfaces;
using AccountService.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AccountService.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor để tự động xử lý Soft Delete
/// Chặn lệnh Remove() và chuyển thành Update với IsDeleted = true
/// </summary>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    public SoftDeleteInterceptor(ICurrentUserService currentUserService, IDateTime dateTime)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State == EntityState.Deleted)
            {
                // Chuyển Delete thành Update
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = _dateTime.UtcNow;
                entry.Entity.DeletedByUserId = _currentUserService.UserId;
            }
        }

        // Xử lý AuditableEntity
        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = _dateTime.UtcNow;
                entry.Entity.CreatedBy = _currentUserService.UserName;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedAt = _dateTime.UtcNow;
                entry.Entity.LastModifiedBy = _currentUserService.UserName;
            }
        }
    }
}