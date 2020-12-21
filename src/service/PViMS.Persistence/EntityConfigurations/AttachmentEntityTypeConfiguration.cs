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

            configuration.Property(c => c.Content)
                .IsRequired(true);

            configuration.Property(c => c.Description)
                .HasMaxLength(100)
                .IsRequired(false);

            configuration.Property(c => c.FileName)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.Size)
                .IsRequired(true);

            configuration.Property(c => c.Created)
                .IsRequired(true);

            configuration.Property(c => c.LastUpdated)
                .IsRequired(false);

            configuration.HasOne(p => p.AttachmentType)
                .WithMany()
                .HasForeignKey("AttachmentType_Id")
                .IsRequired(true);

            configuration.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedBy_Id");

            configuration.HasOne(p => p.Encounter)
                .WithMany()
                .HasForeignKey("Encounter_Id")
                .IsRequired(false);

            configuration.HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey("Patient_Id")
                .IsRequired(false);

            configuration.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey("UpdatedBy_Id");

            configuration.Property(c => c.Archived)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.ArchivedDate)
                .IsRequired(false);

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200)
                .IsRequired(false);

            configuration.HasOne(p => p.AuditUser)
                .WithMany()
                .HasForeignKey("AuditUser_Id");

            configuration.HasOne(p => p.ActivityExecutionStatusEvent)
                .WithMany()
                .HasForeignKey("ActivityExecutionStatusEvent_Id")
                .IsRequired(false);
        }
    }
}
