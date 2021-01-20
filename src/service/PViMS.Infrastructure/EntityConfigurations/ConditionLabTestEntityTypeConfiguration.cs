using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ConditionLabTestEntityTypeConfiguration : IEntityTypeConfiguration<ConditionLabTest>
    {
        public void Configure(EntityTypeBuilder<ConditionLabTest> configuration)
        {
            configuration.ToTable("ConditionLabTest");

            configuration.HasKey(e => e.Id);

            configuration.HasOne(c => c.Condition)
                .WithMany()
                .HasForeignKey("Condition_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.LabTest)
                .WithMany()
                .HasForeignKey("LabTest_Id")
                .IsRequired(true);

            configuration.HasIndex("Condition_Id", "LabTest_Id").IsUnique(true);
        }
    }
}
