using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetCategoryConditionEntityTypeConfiguration : IEntityTypeConfiguration<DatasetCategoryCondition>
    {
        public void Configure(EntityTypeBuilder<DatasetCategoryCondition> configuration)
        {
            configuration.ToTable("DatasetCategoryCondition");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ConditionId)
                .IsRequired()
                .HasColumnName("Condition_Id");

            configuration.Property(e => e.DatasetCategoryId)
                .IsRequired()
                .HasColumnName("DatasetCategory_Id");

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.DatasetCategoryConditions)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetCategoryCondition_dbo.Condition_Condition_Id");

            configuration.HasOne(d => d.DatasetCategory)
                .WithMany(p => p.DatasetCategoryConditions)
                .HasForeignKey(d => d.DatasetCategoryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetCategoryCondition_dbo.DatasetCategory_DatasetCategory_Id");

            configuration.HasIndex(e => new { e.ConditionId, e.DatasetCategoryId }).IsUnique(true);
            configuration.HasIndex(e => e.ConditionId, "IX_Condition_Id");
            configuration.HasIndex(e => e.DatasetCategoryId, "IX_DatasetCategory_Id");
        }
    }
}
