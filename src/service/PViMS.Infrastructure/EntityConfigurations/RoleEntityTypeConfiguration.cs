using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Accounts;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> configuration)
        {
            configuration.ToTable("Role");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(30);

            configuration.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(30);

            configuration.HasIndex("Name").IsUnique(true);
            configuration.HasIndex("Key").IsUnique(true);
        }
    }
}
