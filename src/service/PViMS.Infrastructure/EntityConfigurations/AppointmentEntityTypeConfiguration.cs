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
                .IsRequired(true);

            configuration.Property(c => c.Reason)
                .HasMaxLength(250)
                .IsRequired(true);

            configuration.Property(c => c.DNA)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.Cancelled)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.CancellationReason)
                .HasMaxLength(250)
                .IsRequired(false);

            configuration.Property(c => c.Created)
                .IsRequired(true);

            configuration.Property(c => c.LastUpdated)
                .IsRequired(false);

            configuration.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedBy_Id");

            configuration.HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey("Patient_Id")
                .IsRequired(true);

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

            configuration.HasIndex("AppointmentDate").IsUnique(false);
        }
    }
}
