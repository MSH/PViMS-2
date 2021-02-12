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
                .IsRequired(true)
                .HasMaxLength(1000);

            configuration.Property(c => c.Active)
                .IsRequired()
                .HasDefaultValue(true);

            configuration.Property(e => e.MedicationFormId)
                .IsRequired()
                .HasColumnName("MedicationForm_Id");

            configuration.HasOne(d => d.MedicationForm)
                .WithMany(p => p.Concepts)
                .HasForeignKey(d => d.MedicationFormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.Concept_dbo.MedicationForm_MedicationForm_Id");

            configuration.HasIndex(new string[] { "ConceptName", "MedicationForm_Id" }).IsUnique(true);
            configuration.HasIndex(e => e.MedicationFormId, "IX_MedicationForm_Id");
        }
    }
}
