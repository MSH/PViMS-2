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

            configuration.HasOne(c => c.Condition)
                .WithMany()
                .HasForeignKey("Condition_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.DatasetCategoryElement)
                .WithMany()
                .HasForeignKey("DatasetCategoryElement_Id")
                .IsRequired(true);

            configuration.HasIndex("Condition_Id", "DatasetCategoryElement_Id").IsUnique(true);
        }
    }
}
