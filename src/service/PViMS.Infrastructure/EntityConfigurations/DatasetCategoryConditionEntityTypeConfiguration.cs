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

            configuration.HasOne(c => c.Condition)
                .WithMany()
                .HasForeignKey("Condition_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.DatasetCategory)
                .WithMany()
                .HasForeignKey("DatasetCategory_Id")
                .IsRequired(true);

            configuration.HasIndex("Condition_Id", "DatasetCategory_Id").IsUnique(true);
        }
    }
}
