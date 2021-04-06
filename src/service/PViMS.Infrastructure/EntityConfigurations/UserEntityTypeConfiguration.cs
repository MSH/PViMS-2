using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Accounts;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> configuration)
        {
            configuration.ToTable("User");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.CurrentContext)
                .HasMaxLength(20);

            configuration.Property(e => e.Email)
                .HasMaxLength(256);

            configuration.Property(e => e.EmailConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(e => e.EulaAcceptanceDate)
                .HasColumnType("datetime");

            configuration.Property(e => e.LockoutEndDateUtc)
                .HasColumnType("datetime");

            configuration.Property(e => e.PhoneNumberConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(e => e.TwoFactorEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(e => e.LockoutEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValue(true);

            configuration.Property(e => e.AllowDatasetDownload)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(e => e.IdentityId)
                .IsRequired();

            configuration.Property(e => e.AccessFailedCount)
                .IsRequired();

            configuration.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(256);

            configuration.Property(c => c.UserType)
                .HasConversion(x => (int)x, x => (UserType)x);

            configuration.HasIndex("UserName").IsUnique(true);
        }
    }
}
