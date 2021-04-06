﻿using Microsoft.EntityFrameworkCore;
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

            configuration.Property(e => e.ConceptId)
                .IsRequired()
                .HasColumnName("Concept_Id");

            configuration.Property(c => c.Ingredient)
                .IsRequired()
                .HasMaxLength(200);

            configuration.Property(c => c.Strength)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(c => c.Active)
                .IsRequired()
                .HasDefaultValue(true);

            configuration.HasOne(d => d.Concept)
                .WithMany(p => p.ConceptIngredients)
                .HasForeignKey(d => d.ConceptId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ConceptIngredient_dbo.Concept_Concept_Id");

            configuration.HasIndex(e => new { e.ConceptId, e.Ingredient, e.Strength }).IsUnique(true);
            configuration.HasIndex(e => e.ConceptId, "IX_Concept_Id");
        }
    }
}
