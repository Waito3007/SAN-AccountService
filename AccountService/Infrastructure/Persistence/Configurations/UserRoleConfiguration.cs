using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AccountService.Domain.Entities;

namespace AccountService.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core Configuration cho UserRole Entity (Many-to-Many)
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        // Composite Primary Key
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Property(ur => ur.AssignedAt)
            .IsRequired();

        // Relationships đã được định nghĩa ở UserConfiguration và RoleConfiguration
    }
}

