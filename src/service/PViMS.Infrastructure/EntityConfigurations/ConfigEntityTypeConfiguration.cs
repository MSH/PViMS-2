using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ConfigEntityTypeConfiguration : IEntityTypeConfiguration<Config>
    {
        public void Configure(EntityTypeBuilder<Config> configuration)
        {
            configuration.ToTable("Config");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ConfigType)
                .HasConversion(x => (int)x, x => (ConfigType)x);

            configuration.Property(c => c.ConfigValue)
                .HasMaxLength(100)
                .IsRequired(true);

            configuration.Property(c => c.Created)
                .IsRequired(true);

            configuration.Property(c => c.LastUpdated)
                .IsRequired(false);

            configuration.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedBy_Id");

            configuration.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey("UpdatedBy_Id");

            configuration.HasIndex("ConfigType").IsUnique(false);
        }
    }
}
