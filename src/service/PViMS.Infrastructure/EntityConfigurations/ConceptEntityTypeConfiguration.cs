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
                .IsRequired()
                .HasMaxLength(1000);

            configuration.Property(c => c.Active)
                .IsRequired();

            configuration.Property(e => e.MedicationFormId)
                .IsRequired()
                .HasColumnName("MedicationForm_Id");

            configuration.HasOne(d => d.MedicationForm)
                .WithMany(p => p.Concepts)
                .HasForeignKey(d => d.MedicationFormId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasIndex(e => new { e.ConceptName, e.MedicationFormId }).IsUnique(true);
            configuration.HasIndex(e => e.MedicationFormId);
        }
    }
}
