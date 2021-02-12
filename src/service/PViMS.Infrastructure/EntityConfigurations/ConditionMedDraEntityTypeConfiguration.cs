using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ConditionMedDraEntityTypeConfiguration : IEntityTypeConfiguration<ConditionMedDra>
    {
        public void Configure(EntityTypeBuilder<ConditionMedDra> configuration)
        {
            configuration.ToTable("ConditionMedDra");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ConditionId)
                .IsRequired()
                .HasColumnName("Condition_Id");

            configuration.Property(e => e.TerminologyMedDraId)
                .IsRequired()
                .HasColumnName("TerminologyMedDra_Id");

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.ConditionMedDras)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ConditionMedDra_dbo.Condition_Condition_Id");

            configuration.HasOne(d => d.TerminologyMedDra)
                .WithMany(p => p.ConditionMedDras)
                .HasForeignKey(d => d.TerminologyMedDraId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ConditionMedDra_dbo.TerminologyMedDra_TerminologyMedDra_Id");

            configuration.HasIndex(new string[] { "Condition_Id", "TerminologyMedDra_Id" }).IsUnique(true);
            configuration.HasIndex(e => e.ConditionId, "IX_Condition_Id");
            configuration.HasIndex(e => e.TerminologyMedDraId, "IX_TerminologyMedDra_Id");
        }
    }
}
