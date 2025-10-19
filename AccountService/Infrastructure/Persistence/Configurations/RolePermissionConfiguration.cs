using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AccountService.Domain.Entities;

namespace AccountService.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core Configuration cho RolePermission Entity (Many-to-Many)
/// </summary>
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        // Composite Primary Key
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.Property(rp => rp.AssignedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
