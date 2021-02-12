using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PatientClinicalEventEntityTypeConfiguration : IEntityTypeConfiguration<PatientClinicalEvent>
    {
        public void Configure(EntityTypeBuilder<PatientClinicalEvent> configuration)
        {
            configuration.ToTable("PatientClinicalEvent");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedDate)
                .HasColumnType("datetime");

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.CustomAttributesXmlSerialised)
                .HasColumnType("xml");

            configuration.Property(e => e.EncounterId)
                .HasColumnName("Encounter_Id");

            configuration.Property(e => e.OnsetDate)
                .HasColumnType("date");

            configuration.Property(e => e.PatientClinicalEventGuid)
                .IsRequired()
                .HasDefaultValueSql("(newid())");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.ResolutionDate)
                .HasColumnType("date");

            configuration.Property(e => e.SourceDescription)
                .HasMaxLength(500);

            configuration.Property(e => e.SourceTerminologyMedDraId)
                .HasColumnName("SourceTerminologyMedDra_Id");

            configuration.Property(e => e.TerminologyMedDraId1)
                .HasColumnName("TerminologyMedDra_Id1");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientClinicalEvents)
                .HasForeignKey(d => d.AuditUserId)
                .HasConstraintName("FK_dbo.PatientClinicalEvent_dbo.User_AuditUser_Id");

            configuration.HasOne(d => d.Encounter)
                .WithMany(p => p.PatientClinicalEvents)
                .HasForeignKey(d => d.EncounterId)
                .HasConstraintName("FK_dbo.PatientClinicalEvent_dbo.Encounter_Encounter_Id");

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientClinicalEvents)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.PatientClinicalEvent_dbo.Patient_Patient_Id");

            configuration.HasOne(d => d.SourceTerminologyMedDra)
                .WithMany(p => p.PatientClinicalEvents)
                .HasForeignKey(d => d.SourceTerminologyMedDraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.PatientClinicalEvent_dbo.TerminologyMedDra_SourceTerminologyMedDra_Id");

            configuration.HasIndex(e => e.AuditUserId, "IX_AuditUser_Id");
            configuration.HasIndex(e => e.EncounterId, "IX_Encounter_Id");
            configuration.HasIndex(e => e.PatientId, "IX_Patient_Id");
            configuration.HasIndex(e => e.SourceTerminologyMedDraId, "IX_SourceTerminologyMedDra_Id");
        }
    }
}
