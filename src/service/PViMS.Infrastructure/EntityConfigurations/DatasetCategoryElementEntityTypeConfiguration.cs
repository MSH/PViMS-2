using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetCategoryElementEntityTypeConfiguration : IEntityTypeConfiguration<DatasetCategoryElement>
    {
        public void Configure(EntityTypeBuilder<DatasetCategoryElement> configuration)
        {
            configuration.ToTable("DatasetCategoryElement");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.FieldOrder)
                .IsRequired(true);

            configuration.HasOne(c => c.DatasetCategory)
                .WithMany()
                .HasForeignKey("DatasetCategory_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.DatasetElement)
                .WithMany()
                .HasForeignKey("DatasetElement_Id")
                .IsRequired(true);

            configuration.Property(c => c.Acute)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.Chronic)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.UID)
                .HasMaxLength(10)
                .IsRequired(false);

            configuration.Property(c => c.System)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.Property(c => c.Public)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.FriendlyName)
                .HasMaxLength(150)
                .IsRequired(false);

            configuration.Property(c => c.Help)
                .HasMaxLength(350)
                .IsRequired(false);

            configuration.HasMany(c => c.Conditions)
               .WithOne()
               .HasForeignKey("DatasetCategoryElement_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.SourceMappings)
               .WithOne()
               .HasForeignKey("SourceElement_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.DestinationMappings)
               .WithOne()
               .HasForeignKey("DestinationElement_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("DatasetCategory_Id", "DatasetElement_Id").IsUnique(true);
        }
    }
}
