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

            configuration.Property(c => c.CohortName)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.CohortCode)
                .HasMaxLength(5)
                .IsRequired(true);

            configuration.Property(c => c.LastPatientNo)
                .HasDefaultValue(0)
                .IsRequired(true);

            configuration.Property(c => c.StartDate)
                .IsRequired(true);

            configuration.Property(c => c.FinishDate)
                .IsRequired(false);

            configuration.Property(c => c.MinEnrolment)
                .HasDefaultValue(0)
                .IsRequired(true);

            configuration.Property(c => c.MaxEnrolment)
                .HasDefaultValue(0)
                .IsRequired(true);

            configuration.HasOne(c => c.Condition)
                .WithMany()
                .HasForeignKey("Condition_Id")
                .IsRequired(false);

            configuration.HasMany(c => c.CohortGroupEnrolments)
               .WithOne()
               .HasForeignKey("CohortGroup_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.EncounterTypeWorkPlans)
               .WithOne()
               .HasForeignKey("CohortGroup_Id")
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

            configuration.HasIndex("CohortName").IsUnique(true);
            configuration.HasIndex("CohortCode").IsUnique(true);
        }
    }
}
