using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetMappingValueEntityTypeConfiguration : IEntityTypeConfiguration<DatasetMappingValue>
    {
        public void Configure(EntityTypeBuilder<DatasetMappingValue> configuration)
        {
            configuration.ToTable("DatasetMappingValue");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.SourceValue)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.MappingId)
                .IsRequired()
                .HasColumnName("Mapping_Id");

            configuration.Property(c => c.DestinationValue)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.SubMappingId)
                .HasColumnName("SubMapping_Id");

            configuration.Property(c => c.Active)
                .IsRequired();

            configuration.HasOne(d => d.Mapping)
                .WithMany(p => p.DatasetMappingValues)
                .HasForeignKey(d => d.MappingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetMappingValue_dbo.DatasetMapping_Mapping_Id");

            configuration.HasOne(d => d.SubMapping)
                .WithMany(p => p.DatasetMappingValues)
                .HasForeignKey(d => d.SubMappingId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.DatasetMappingValue_dbo.DatasetMappingSub_SubMapping_Id");

            configuration.HasIndex(e => e.MappingId, "IX_Mapping_Id");
            configuration.HasIndex(e => e.SubMappingId, "IX_SubMapping_Id");
        }
    }
}
