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
                .IsRequired()
                .HasDefaultValue(true);

            configuration.Property(c => c.Acute)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.Chronic)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.Public)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.Dataset)
                .WithMany(p => p.DatasetCategories)
                .HasForeignKey(d => d.DatasetId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetCategory_dbo.Dataset_Dataset_Id1");

            configuration.HasIndex(new string[] { "Dataset_Id", "DatasetCategoryName" }).IsUnique(true);
            configuration.HasIndex(e => e.DatasetId, "IX_Dataset_Id");
        }
    }
}
