using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PViMS.Core.ValueTypes;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetMappingSubEntityTypeConfiguration : IEntityTypeConfiguration<DatasetMappingSub>
    {
        public void Configure(EntityTypeBuilder<DatasetMappingSub> configuration)
        {
            configuration.ToTable("DatasetMappingSub");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.PropertyPath)
                .IsRequired(false);

            configuration.Property(c => c.Property)
                .IsRequired(false);

            configuration.Property(c => c.MappingType)
                .HasConversion(x => (int)x, x => (MappingType)x);

            configuration.Property(c => c.MappingOption)
                .IsRequired(false);

            configuration.HasOne(c => c.DestinationElement)
                .WithMany()
                .HasForeignKey("DestinationElement_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.Mapping)
                .WithMany()
                .HasForeignKey("Mapping_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.SourceElement)
                .WithMany()
                .HasForeignKey("SourceElement_Id");

            configuration.HasMany(c => c.Values)
               .WithOne()
               .HasForeignKey("SubMapping_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
