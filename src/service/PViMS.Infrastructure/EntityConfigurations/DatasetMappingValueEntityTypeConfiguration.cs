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
                .HasMaxLength(100)
                .IsRequired(true);

            configuration.Property(c => c.DestinationValue)
                .HasMaxLength(100)
                .IsRequired(true);

            configuration.Property(c => c.Active)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.HasOne(c => c.Mapping)
                .WithMany()
                .HasForeignKey("Mapping_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.SubMapping)
                .WithMany()
                .HasForeignKey("SubMapping_Id");
        }
    }
}
