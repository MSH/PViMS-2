using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PatientFacilityEntityTypeConfiguration : IEntityTypeConfiguration<PatientFacility>
    {
        public void Configure(EntityTypeBuilder<PatientFacility> configuration)
        {
            configuration.ToTable("PatientFacility");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedDate)
                .HasColumnType("datetime");

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.EnrolledDate)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.FacilityId)
                .IsRequired()
                .HasColumnName("Facility_Id");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientFacilities)
                .HasForeignKey(d => d.AuditUserId)
                .HasConstraintName("FK_dbo.PatientFacility_dbo.User_AuditUser_Id");

            configuration.HasOne(d => d.Facility)
                .WithMany(p => p.PatientFacilities)
                .HasForeignKey(d => d.FacilityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.PatientFacility_dbo.Facility_Facility_Id");

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientFacilities)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.PatientFacility_dbo.Patient_Patient_Id");

            configuration.HasIndex(new string[] { "Patient_Id", "Facility_Id" }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId, "IX_AuditUser_Id");
            configuration.HasIndex(e => e.FacilityId, "IX_Facility_Id");
            configuration.HasIndex(e => e.PatientId, "IX_Patient_Id");
        }
    }
}
