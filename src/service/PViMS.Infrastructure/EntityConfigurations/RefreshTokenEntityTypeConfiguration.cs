using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Accounts;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class RefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> configuration)
        {
            configuration.ToTable("RefreshToken");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Expires)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("User_Id");

            configuration.HasOne(d => d.User)
                .WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.RefreshToken_dbo.User_User_Id");

            configuration.HasIndex(e => e.UserId, "IX_User_Id");
        }
    }
}
