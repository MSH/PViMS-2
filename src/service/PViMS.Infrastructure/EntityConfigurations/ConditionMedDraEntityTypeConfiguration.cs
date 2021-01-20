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

            configuration.HasOne(c => c.Condition)
                .WithMany()
                .HasForeignKey("Condition_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.TerminologyMedDra)
                .WithMany()
                .HasForeignKey("TerminologyMedDra_Id")
                .IsRequired(true);

            configuration.HasIndex("Condition_Id", "TerminologyMedDra_Id").IsUnique(true);
        }
    }
}
