using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetMappingSubEntityTypeConfiguration : IEntityTypeConfiguration<DatasetMappingSub>
    {
        public void Configure(EntityTypeBuilder<DatasetMappingSub> configuration)
        {
            configuration.ToTable("DatasetMappingSub");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DestinationElementId)
                .IsRequired()
                .HasColumnName("DestinationElement_Id");

            configuration.Property(e => e.MappingId)
                .IsRequired()
                .HasColumnName("Mapping_Id");

            configuration.Property(e => e.SourceElementId)
                .HasColumnName("SourceElement_Id");

            configuration.Property(c => c.MappingType)
                .HasConversion(x => (int)x, x => (MappingType)x);

            configuration.HasOne(d => d.DestinationElement)
                .WithMany(p => p.DatasetMappingSubDestinationElements)
                .HasForeignKey(d => d.DestinationElementId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetMappingSub_dbo.DatasetElementSub_DestinationElement_Id");

            configuration.HasOne(d => d.Mapping)
                .WithMany(p => p.SubMappings)
                .HasForeignKey(d => d.MappingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetMappingSub_dbo.DatasetMapping_Mapping_Id");

            configuration.HasOne(d => d.SourceElement)
                .WithMany(p => p.DatasetMappingSubSourceElements)
                .HasForeignKey(d => d.SourceElementId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetMappingSub_dbo.DatasetElementSub_SourceElement_Id");

            configuration.HasIndex(e => e.DestinationElementId, "IX_DestinationElement_Id");
            configuration.HasIndex(e => e.MappingId, "IX_Mapping_Id");
            configuration.HasIndex(e => e.SourceElementId, "IX_SourceElement_Id");
        }
    }
}
