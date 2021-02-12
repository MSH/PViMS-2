﻿using Microsoft.EntityFrameworkCore;
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

            configuration.Property(e => e.ConceptId)
                .IsRequired()
                .HasColumnName("Concept_Id");

            configuration.Property(e => e.ConditionId)
                .HasColumnName("Condition_Id");

            configuration.Property(e => e.ProductId)
                .HasColumnName("Product_Id");

            configuration.HasOne(d => d.Concept)
                .WithMany(p => p.ConditionMedications)
                .HasForeignKey(d => d.ConceptId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ConditionMedication_dbo.Concept_Concept_Id");

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.ConditionMedications)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ConditionMedication_dbo.Condition_Condition_Id");

            configuration.HasOne(d => d.Product)
                .WithMany(p => p.ConditionMedications)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ConditionMedication_dbo.Product_Product_Id");

            configuration.HasIndex(new string[] { "Condition_Id", "Concept_Id", "Product_Id" }).IsUnique(true);
            configuration.HasIndex(e => e.ConceptId, "IX_Concept_Id");
            configuration.HasIndex(e => e.ConditionId, "IX_Condition_Id");
            configuration.HasIndex(e => e.ProductId, "IX_Product_Id");
        }
    }
}