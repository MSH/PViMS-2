using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ConceptEntityTypeConfiguration : IEntityTypeConfiguration<Concept>
    {
        public void Configure(EntityTypeBuilder<Concept> configuration)
        {
            configuration.ToTable("Concept");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ConceptName)
                .HasMaxLength(1000)
                .IsRequired(true);

            configuration.Property(c => c.Active)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.HasOne(c => c.MedicationForm)
                .WithMany()
                .HasForeignKey("MedicationForm_Id")
                .IsRequired(true);

            configuration.HasMany(c => c.ConceptIngredients)
               .WithOne()
               .HasForeignKey("Concept_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.Products)
               .WithOne()
               .HasForeignKey("Concept_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("ConceptName", "MedicationForm_Id").IsUnique(true);
        }
    }
}
