using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetCategoryEntityTypeConfiguration : IEntityTypeConfiguration<DatasetCategory>
    {
        public void Configure(EntityTypeBuilder<DatasetCategory> configuration)
        {
            configuration.ToTable("DatasetCategory");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.DatasetCategoryName)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.CategoryOrder)
                .IsRequired(true);

            configuration.HasOne(c => c.Dataset)
                .WithMany()
                .HasForeignKey("Dataset_Id")
                .IsRequired(true);

            configuration.Property(c => c.UID)
                .HasMaxLength(10)
                .IsRequired(false);

            configuration.Property(c => c.System)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.Property(c => c.Acute)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.Chronic)
                .HasDefaultValue(false)
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

            configuration.HasMany(c => c.DatasetCategoryElements)
               .WithOne()
               .HasForeignKey("DatasetCategory_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.Conditions)
               .WithOne()
               .HasForeignKey("DatasetCategory_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("Dataset_Id", "DatasetCategoryName").IsUnique(true);
        }
    }
}
