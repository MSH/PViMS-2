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
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.DatasetId)
                .IsRequired()
                .HasColumnName("Dataset_Id");

            configuration.Property(c => c.CategoryOrder)
                .IsRequired(true);

            configuration.HasOne(c => c.Dataset)
                .WithMany()
                .HasForeignKey("Dataset_Id")
                .IsRequired(true);

            configuration.Property(c => c.FriendlyName)
                .HasMaxLength(150);

            configuration.Property(c => c.Help)
                .HasMaxLength(350);

            configuration.Property(c => c.Uid)
                .HasMaxLength(10)
                .HasColumnName("UID");

            configuration.Property(c => c.System)
                .IsRequired();

            configuration.Property(c => c.Acute)
                .IsRequired();

            configuration.Property(c => c.Chronic)
                .IsRequired();

            configuration.Property(c => c.Public)
                .IsRequired();

            configuration.HasOne(d => d.Dataset)
                .WithMany(p => p.DatasetCategories)
                .HasForeignKey(d => d.DatasetId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(new string[] { "Dataset_Id", "DatasetCategoryName" }).IsUnique(true);
            configuration.HasIndex(e => e.DatasetId);
        }
    }
}
