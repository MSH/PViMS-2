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

            configuration.Property(e => e.ConditionId)
                .IsRequired()
                .HasColumnName("Condition_Id");

            configuration.Property(e => e.LabTestId)
                .IsRequired()
                .HasColumnName("LabTest_Id");

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.ConditionLabTests)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ConditionLabTest_dbo.Condition_Condition_Id");

            configuration.HasOne(d => d.LabTest)
                .WithMany(p => p.ConditionLabTests)
                .HasForeignKey(d => d.LabTestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ConditionLabTest_dbo.LabTest_LabTest_Id");

            configuration.HasIndex(e => new { e.ConditionId, e.LabTestId }).IsUnique(true);
            configuration.HasIndex(e => e.ConditionId, "IX_Condition_Id");
            configuration.HasIndex(e => e.LabTestId, "IX_LabTest_Id");
        }
    }
}
