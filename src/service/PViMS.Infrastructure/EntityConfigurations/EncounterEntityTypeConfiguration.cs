using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class EncounterEntityTypeConfiguration : IEntityTypeConfiguration<Encounter>
    {
        public void Configure(EntityTypeBuilder<Encounter> configuration)
        {
            configuration.ToTable("Encounter");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedDate)
                .HasColumnType("datetime");

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(c => c.EncounterDate)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.EncounterTypeId)
                .IsRequired()
                .HasColumnName("EncounterType_Id");

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(c => c.EncounterGuid)
                .IsRequired()
                .HasDefaultValueSql("newid()");

            configuration.Property(c => c.Discharged)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.PregnancyId)
                .HasColumnName("Pregnancy_Id");

            configuration.Property(e => e.PriorityId)
                .IsRequired()
                .HasColumnName("Priority_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.Encounters)
                .HasForeignKey(d => d.AuditUserId)
                .HasConstraintName("FK_dbo.Encounter_dbo.User_AuditUser_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.EncounterCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.Encounter_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.EncounterType)
                .WithMany(p => p.Encounters)
                .HasForeignKey(d => d.EncounterTypeId)
                .HasConstraintName("FK_dbo.Encounter_dbo.EncounterType_EncounterType_Id");

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.Encounters)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK_dbo.Encounter_dbo.Patient_Patient_Id");

            configuration.HasOne(d => d.Pregnancy)
                .WithMany(p => p.Encounters)
                .HasForeignKey(d => d.PregnancyId)
                .HasConstraintName("FK_dbo.Encounter_dbo.Pregnancy_Pregnancy_Id");

            configuration.HasOne(d => d.Priority)
                .WithMany(p => p.Encounters)
                .HasForeignKey(d => d.PriorityId)
                .HasConstraintName("FK_dbo.Encounter_dbo.Priority_Priority_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.EncounterUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.Encounter_dbo.User_UpdatedBy_Id");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasIndex(e => e.EncounterDate).IsUnique(false);
            configuration.HasIndex(e => new { e.PatientId, e.EncounterDate }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId, "IX_AuditUser_Id");
            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.EncounterTypeId, "IX_EncounterType_Id");
            configuration.HasIndex(e => e.PatientId, "IX_Patient_Id");
            configuration.HasIndex(e => e.PregnancyId, "IX_Pregnancy_Id");
            configuration.HasIndex(e => e.PriorityId, "IX_Priority_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
