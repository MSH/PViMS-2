using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class CohortGroupEnrolmentEntityTypeConfiguration : IEntityTypeConfiguration<CohortGroupEnrolment>
    {
        public void Configure(EntityTypeBuilder<CohortGroupEnrolment> configuration)
        {
            configuration.ToTable("CohortGroupEnrolment");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedDate)
                .HasColumnType("datetime");

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.CohortGroupId)
                .IsRequired()
                .HasColumnName("CohortGroup_Id");

            configuration.Property(c => c.DeenroledDate)
                .HasColumnType("datetime");

            configuration.Property(c => c.EnroledDate)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.CohortGroupEnrolments)
                .HasForeignKey(d => d.AuditUserId)
                .HasConstraintName("FK_dbo.CohortGroupEnrolment_dbo.User_AuditUser_Id");

            configuration.HasOne(d => d.CohortGroup)
                .WithMany(p => p.CohortGroupEnrolments)
                .HasForeignKey(d => d.CohortGroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.CohortGroupEnrolment_dbo.CohortGroup_CohortGroup_Id");

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.CohortEnrolments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.CohortGroupEnrolment_dbo.Patient_Patient_Id");

            configuration.HasIndex(e => e.AuditUserId, "IX_AuditUser_Id");
            configuration.HasIndex(e => e.CohortGroupId, "IX_CohortGroup_Id");
            configuration.HasIndex(e => e.PatientId, "IX_Patient_Id");
        }
    }
}
