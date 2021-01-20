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

            configuration.HasOne(c => c.CohortGroup)
                .WithMany()
                .HasForeignKey("CohortGroup_Id")
                .IsRequired(false);

            configuration.HasOne(c => c.EncounterType)
                .WithMany()
                .HasForeignKey("EncounterType_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.WorkPlan)
                .WithMany()
                .HasForeignKey("WorkPlan_Id")
                .IsRequired(true);

            configuration.HasIndex("CohortGroup_Id", "EncounterType_Id", "WorkPlan_Id").IsUnique(true);
        }
    }
}
