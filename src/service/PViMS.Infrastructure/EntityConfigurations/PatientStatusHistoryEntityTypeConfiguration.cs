using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PatientStatusHistoryEntityTypeConfiguration : IEntityTypeConfiguration<PatientStatusHistory>
    {
        public void Configure(EntityTypeBuilder<PatientStatusHistory> configuration)
        {
            configuration.ToTable("PatientStatusHistory");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ArchivedDate)
                .HasColumnType("datetime");

            configuration.Property(e => e.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.Comments)
                .HasMaxLength(100);

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.EffectiveDate)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.PatientStatusId)
                .IsRequired()
                .HasColumnName("PatientStatus_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientStatusHistories)
                .HasForeignKey(d => d.AuditUserId)
                .HasConstraintName("FK_dbo.PatientStatusHistory_dbo.User_AuditUser_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.PatientStatusHistoryCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.PatientStatusHistory_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientStatusHistories)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.PatientStatusHistory_dbo.Patient_Patient_Id");

            configuration.HasOne(d => d.PatientStatus)
                .WithMany(p => p.PatientStatusHistories)
                .HasForeignKey(d => d.PatientStatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.PatientStatusHistory_dbo.PatientStatus_PatientStatus_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.PatientStatusHistoryUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.PatientStatusHistory_dbo.User_UpdatedBy_Id");

            configuration.HasIndex(new string[] { "Patient_Id", "PatientStatus_Id" }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId, "IX_AuditUser_Id");
            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.PatientStatusId, "IX_PatientStatus_Id");
            configuration.HasIndex(e => e.PatientId, "IX_Patient_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
