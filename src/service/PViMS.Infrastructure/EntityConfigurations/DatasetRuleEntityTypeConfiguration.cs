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

            configuration.Property(e => e.DatasetElementId)
                .HasColumnName("DatasetElement_Id");

            configuration.Property(e => e.DatasetId)
                .HasColumnName("Dataset_Id");

            configuration.Property(c => c.RuleType)
                .HasConversion(x => (int)x, x => (DatasetRuleType)x);

            configuration.Property(c => c.RuleActive)
                .IsRequired();

            configuration.HasOne(d => d.DatasetElement)
                .WithMany(p => p.DatasetRules)
                .HasForeignKey(d => d.DatasetElementId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetRule_dbo.DatasetElement_DatasetElement_Id");

            configuration.HasOne(d => d.Dataset)
                .WithMany(p => p.DatasetRules)
                .HasForeignKey(d => d.DatasetId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetRule_dbo.Dataset_Dataset_Id");

            configuration.HasIndex(e => e.DatasetElementId, "IX_DatasetElement_Id");
            configuration.HasIndex(e => e.DatasetId, "IX_Dataset_Id");
        }
    }
}
