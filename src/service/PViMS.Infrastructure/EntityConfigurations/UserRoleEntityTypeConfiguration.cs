using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Accounts;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> configuration)
        {
            configuration.ToTable("UserRole");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.RoleId)
                .IsRequired()
                .HasColumnName("Role_Id");

            configuration.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("User_Id");

            configuration.HasOne(d => d.Role)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.UserRole_dbo.Role_Role_Id");

            configuration.HasOne(d => d.User)
                .WithMany(p => p.Roles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.UserRole_dbo.User_User_Id");

            configuration.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique(true);
            configuration.HasIndex(e => e.RoleId, "IX_Role_Id");
            configuration.HasIndex(e => e.UserId, "IX_User_Id");
        }
    }
}
