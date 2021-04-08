using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class AttachmentEntityTypeConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> configuration)
        {
            configuration.ToTable("Attachment");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ActivityExecutionStatusEventId)
                .HasColumnName("ActivityExecutionStatusEvent_Id");

            configuration.Property(e => e.ArchivedDate)
                .HasColumnType("datetime");

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AttachmentTypeId)
                .IsRequired()
                .HasColumnName("AttachmentType_Id");

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(c => c.Content)
                .IsRequired();

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(c => c.Description)
                .HasMaxLength(100);

            configuration.Property(e => e.EncounterId)
                .HasColumnName("Encounter_Id");

            configuration.Property(c => c.FileName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(e => e.PatientId)
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.Size)
                .IsRequired(true);

            configuration.Property(c => c.Archived)
                .HasDefaultValue(false)
                .IsRequired();

            configuration.HasOne(d => d.ActivityExecutionStatusEvent)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.ActivityExecutionStatusEventId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.Attachment_dbo.ActivityExecutionStatusEvent_ActivityExecutionStatusEvent_Id");

            configuration.HasOne(d => d.AttachmentType)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.AttachmentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.Attachment_dbo.AttachmentType_AttachmentType_Id");

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.AuditUserId)
                .HasConstraintName("FK_dbo.Attachment_dbo.User_AuditUser_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.AttachmentCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.Attachment_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.Encounter)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.EncounterId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.Attachment_dbo.Encounter_Encounter_Id");

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.Attachment_dbo.Patient_Patient_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.AttachmentUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.Attachment_dbo.User_UpdatedBy_Id");

            configuration.HasIndex(e => e.ActivityExecutionStatusEventId, "IX_ActivityExecutionStatusEvent_Id");
            configuration.HasIndex(e => e.AttachmentTypeId, "IX_AttachmentType_Id");
            configuration.HasIndex(e => e.AuditUserId, "IX_AuditUser_Id");
            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.EncounterId, "IX_Encounter_Id");
            configuration.HasIndex(e => e.PatientId, "IX_Patient_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
