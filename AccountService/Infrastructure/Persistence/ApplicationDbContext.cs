using Microsoft.EntityFrameworkCore;
using AccountService.Domain.Entities;
using AccountService.Domain.Common;
using System.Reflection;

namespace AccountService.Infrastructure.Persistence;

/// <summary>
/// Application Database Context
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Áp dụng tất cả configurations từ assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global Query Filter cho Soft Delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                // Tạo expression: entity => !entity.IsDeleted
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "entity");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                var filter = System.Linq.Expressions.Expression.Lambda(
                    System.Linq.Expressions.Expression.Not(property), 
                    parameter
                );

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Interceptor sẽ tự động xử lý AuditableEntity và SoftDelete
        return await base.SaveChangesAsync(cancellationToken);
    }
}

