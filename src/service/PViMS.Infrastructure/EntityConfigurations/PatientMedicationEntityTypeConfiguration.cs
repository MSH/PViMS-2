using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PatientMedicationEntityTypeConfiguration : IEntityTypeConfiguration<PatientMedication>
    {
        public void Configure(EntityTypeBuilder<PatientMedication> configuration)
        {
            configuration.ToTable("PatientMedication");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ArchivedDate)
                .HasColumnType("datetime");

            configuration.Property(e => e.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.ConceptId)
                .IsRequired()
                .HasColumnName("Concept_Id");

            configuration.Property(e => e.CustomAttributesXmlSerialised)
                .HasColumnType("xml");

            configuration.Property(e => e.DateEnd)
                .HasColumnType("datetime");

            configuration.Property(e => e.DateStart)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.Dose)
                .HasMaxLength(30);

            configuration.Property(e => e.DoseFrequency)
                .HasMaxLength(30);

            configuration.Property(e => e.DoseUnit)
                .HasMaxLength(10);

            configuration.Property(e => e.MedicationSource)
                .HasMaxLength(200);

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.PatientMedicationGuid)
                .IsRequired()
                .HasDefaultValueSql("(newid())");

            configuration.Property(e => e.ProductId)
                .HasColumnName("Product_Id");

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientMedications)
                .HasForeignKey(d => d.AuditUserId)
                .HasConstraintName("FK_dbo.PatientMedication_dbo.User_AuditUser_Id");

            configuration.HasOne(d => d.Concept)
                .WithMany(p => p.PatientMedications)
                .HasForeignKey(d => d.ConceptId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.PatientMedication_dbo.Concept_Concept_Id");

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientMedications)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.PatientMedication_dbo.Patient_Patient_Id");

            configuration.HasOne(d => d.Product)
                .WithMany(p => p.PatientMedications)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.PatientMedication_dbo.Product_Product_Id");

            configuration.HasIndex(e => new { e.PatientId, e.ConceptId, e.ProductId }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId, "IX_AuditUser_Id");
            configuration.HasIndex(e => e.ConceptId, "IX_Concept_Id");
            configuration.HasIndex(e => e.PatientId, "IX_Patient_Id");
            configuration.HasIndex(e => e.ProductId, "IX_Product_Id");
        }
    }
}
