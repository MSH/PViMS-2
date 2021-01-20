using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ConceptIngredientEntityTypeConfiguration : IEntityTypeConfiguration<ConceptIngredient>
    {
        public void Configure(EntityTypeBuilder<ConceptIngredient> configuration)
        {
            configuration.ToTable("ConceptIngredient");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Ingredient)
                .HasMaxLength(200)
                .IsRequired(true);

            configuration.Property(c => c.Strength)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.Active)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.HasOne(c => c.Concept)
                .WithMany()
                .HasForeignKey("Concept_Id")
                .IsRequired(true);

            configuration.HasIndex("Concept_Id", "Ingredient", "Strength").IsUnique(true);
        }
    }
}
