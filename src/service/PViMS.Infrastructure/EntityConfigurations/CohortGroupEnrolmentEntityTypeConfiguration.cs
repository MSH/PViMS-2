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

            configuration.Property(c => c.EnroledDate)
                .IsRequired(true);

            configuration.HasOne(c => c.CohortGroup)
                .WithMany()
                .HasForeignKey("CohortGroup_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.Patient)
                .WithMany()
                .HasForeignKey("Patient_Id")
                .IsRequired(true);

            configuration.Property(c => c.DeenroledDate)
                .IsRequired(false);

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
        }
    }
}
