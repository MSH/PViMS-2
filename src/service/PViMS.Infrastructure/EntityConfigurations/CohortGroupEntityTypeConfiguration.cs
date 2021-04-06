using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class CohortGroupEntityTypeConfiguration : IEntityTypeConfiguration<CohortGroup>
    {
        public void Configure(EntityTypeBuilder<CohortGroup> configuration)
        {
            configuration.ToTable("CohortGroup");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.CohortCode)
                .IsRequired()
                .HasMaxLength(5);

            configuration.Property(c => c.CohortName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(c => c.LastPatientNo)
                .IsRequired()
                .HasDefaultValue(0);

            configuration.Property(c => c.StartDate)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(c => c.FinishDate)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(c => c.MinEnrolment)
                .IsRequired(true)
                .HasDefaultValue(0);

            configuration.Property(c => c.MaxEnrolment)
                .IsRequired(true)
                .HasDefaultValue(0);

            configuration.Property(e => e.ConditionId)
                .HasColumnName("Condition_Id");

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.CohortGroups)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.CohortGroup_dbo.Condition_Condition_Id");

            configuration.HasIndex(e => e.ConditionId, "IX_Condition_Id");
            configuration.HasIndex("CohortName").IsUnique(true);
            configuration.HasIndex("CohortCode").IsUnique(true);
        }
    }
}
