using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetRuleEntityTypeConfiguration : IEntityTypeConfiguration<DatasetRule>
    {
        public void Configure(EntityTypeBuilder<DatasetRule> configuration)
        {
            configuration.ToTable("DatasetRule");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.RuleType)
                .HasConversion(x => (int)x, x => (DatasetRuleType)x);

            configuration.Property(c => c.RuleActive)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.HasOne(c => c.Dataset)
                .WithMany()
                .HasForeignKey("Dataset_Id");

            configuration.HasOne(c => c.DatasetElement)
                .WithMany()
                .HasForeignKey("DatasetElement_Id");
        }
    }
}
