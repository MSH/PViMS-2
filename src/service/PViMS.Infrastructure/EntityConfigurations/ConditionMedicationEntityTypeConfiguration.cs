using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ConditionMedicationEntityTypeConfiguration : IEntityTypeConfiguration<ConditionMedication>
    {
        public void Configure(EntityTypeBuilder<ConditionMedication> configuration)
        {
            configuration.ToTable("ConditionMedication");

            configuration.HasKey(e => e.Id);

            configuration.HasOne(c => c.Condition)
                .WithMany()
                .HasForeignKey("Condition_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.Concept)
                .WithMany()
                .HasForeignKey("Concept_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey("Product_Id")
                .IsRequired(false);

            configuration.HasIndex("Condition_Id", "Concept_Id", "Product_Id").IsUnique(true);
        }
    }
}
