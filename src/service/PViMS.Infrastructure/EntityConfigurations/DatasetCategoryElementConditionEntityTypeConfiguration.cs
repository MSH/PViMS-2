using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetCategoryElementConditionEntityTypeConfiguration : IEntityTypeConfiguration<DatasetCategoryElementCondition>
    {
        public void Configure(EntityTypeBuilder<DatasetCategoryElementCondition> configuration)
        {
            configuration.ToTable("DatasetCategoryElementCondition");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ConditionId)
                .IsRequired()
                .HasColumnName("Condition_Id");

            configuration.Property(e => e.DatasetCategoryElementId)
                .IsRequired()
                .HasColumnName("DatasetCategoryElement_Id");

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.DatasetCategoryElementConditions)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.DatasetCategoryElementCondition_dbo.Condition_Condition_Id");

            configuration.HasOne(d => d.DatasetCategoryElement)
                .WithMany(p => p.DatasetCategoryElementConditions)
                .HasForeignKey(d => d.DatasetCategoryElementId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetCategoryElementCondition_dbo.DatasetCategoryElement_DatasetCategoryElement_Id");

            configuration.HasIndex(new string[] { "Condition_Id", "DatasetCategoryElement_Id" }).IsUnique(true);
            configuration.HasIndex(e => e.ConditionId, "IX_Condition_Id");
            configuration.HasIndex(e => e.DatasetCategoryElementId, "IX_DatasetCategoryElement_Id");
        }
    }
}
