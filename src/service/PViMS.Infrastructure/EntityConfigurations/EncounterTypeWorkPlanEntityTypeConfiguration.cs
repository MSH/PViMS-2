using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class EncounterTypeWorkPlanEntityTypeConfiguration : IEntityTypeConfiguration<EncounterTypeWorkPlan>
    {
        public void Configure(EntityTypeBuilder<EncounterTypeWorkPlan> configuration)
        {
            configuration.ToTable("EncounterTypeWorkPlan");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.CohortGroupId)
                .HasColumnName("CohortGroup_Id");

            configuration.Property(e => e.EncounterTypeId)
                .IsRequired()
                .HasColumnName("EncounterType_Id");

            configuration.Property(e => e.WorkPlanId)
                .IsRequired()
                .HasColumnName("WorkPlan_Id");

            configuration.HasOne(d => d.CohortGroup)
                .WithMany(p => p.EncounterTypeWorkPlans)
                .HasForeignKey(d => d.CohortGroupId)
                .HasConstraintName("FK_dbo.EncounterTypeWorkPlan_dbo.CohortGroup_CohortGroup_Id");

            configuration.HasOne(d => d.EncounterType)
                .WithMany(p => p.EncounterTypeWorkPlans)
                .HasForeignKey(d => d.EncounterTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.EncounterTypeWorkPlan_dbo.EncounterType_EncounterType_Id");

            configuration.HasOne(d => d.WorkPlan)
                .WithMany(p => p.EncounterTypeWorkPlans)
                .HasForeignKey(d => d.WorkPlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.EncounterTypeWorkPlan_dbo.WorkPlan_WorkPlan_Id");

            configuration.HasIndex(new string[] { "CohortGroup_Id", "EncounterType_Id", "WorkPlan_Id" }).IsUnique(true);
            configuration.HasIndex(e => e.CohortGroupId, "IX_CohortGroup_Id");
            configuration.HasIndex(e => e.EncounterTypeId, "IX_EncounterType_Id");
            configuration.HasIndex(e => e.WorkPlanId, "IX_WorkPlan_Id");
        }
    }
}
