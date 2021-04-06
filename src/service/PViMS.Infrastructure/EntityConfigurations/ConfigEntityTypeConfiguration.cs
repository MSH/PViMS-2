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

            configuration.Property(c => c.ConfigValue)
                .IsRequired(true)
                .HasMaxLength(100);

            configuration.Property(c => c.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(c => c.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.ConfigType)
                .HasConversion(x => (int)x, x => (ConfigType)x);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.ConfigCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.Config_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.ConfigUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.Config_dbo.User_UpdatedBy_Id");

            configuration.HasIndex("ConfigType").IsUnique(false);
            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
