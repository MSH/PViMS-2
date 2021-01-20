using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PViMS.Core.ValueTypes;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetMappingEntityTypeConfiguration : IEntityTypeConfiguration<DatasetMapping>
    {
        public void Configure(EntityTypeBuilder<DatasetMapping> configuration)
        {
            configuration.ToTable("DatasetMapping");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Tag)
                .IsRequired(false);

            configuration.Property(c => c.MappingType)
                .HasConversion(x => (int)x, x => (MappingType)x);

            configuration.Property(c => c.MappingOption)
                .IsRequired(false);

            configuration.HasOne(c => c.DestinationElement)
                .WithMany()
                .HasForeignKey("DestinationElement_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.SourceElement)
                .WithMany()
                .HasForeignKey("SourceElement_Id");

            configuration.Property(c => c.PropertyPath)
                .IsRequired(false);

            configuration.Property(c => c.Property)
                .IsRequired(false);

            configuration.HasMany(c => c.Values)
               .WithOne()
               .HasForeignKey("Mapping_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.SubMappings)
               .WithOne()
               .HasForeignKey("Mapping_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
