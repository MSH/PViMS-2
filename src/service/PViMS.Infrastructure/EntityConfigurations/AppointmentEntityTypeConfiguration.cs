using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class AppointmentEntityTypeConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> configuration)
        {
            configuration.ToTable("Appointment");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.AppointmentDate)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.ArchivedDate)
                .HasColumnType("datetime");

            configuration.Property(e => e.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(c => c.CancellationReason)
                .HasMaxLength(250);

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.Dna)
                .IsRequired()
                .HasColumnName("DNA")
                .HasDefaultValue(false);

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(c => c.Reason)
                .IsRequired()
                .HasMaxLength(250);

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.Cancelled)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.ArchivedDate)
                .HasColumnType("datetime");

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.Appointments)
                .HasForeignKey(d => d.AuditUserId)
                .HasConstraintName("FK_dbo.Appointment_dbo.User_AuditUser_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.AppointmentCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.Appointment_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.Appointment_dbo.Patient_Patient_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.AppointmentUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.Appointment_dbo.User_UpdatedBy_Id");

            configuration.HasIndex("AppointmentDate").IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId, "IX_AuditUser_Id");
            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.PatientId, "IX_Patient_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
