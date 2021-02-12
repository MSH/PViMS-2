using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PatientLabTestEntityTypeConfiguration : IEntityTypeConfiguration<PatientLabTest>
    {
        public void Configure(EntityTypeBuilder<PatientLabTest> configuration)
        {
            configuration.ToTable("PatientLabTest");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedDate)
                .HasColumnType("datetime");

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.CustomAttributesXmlSerialised)
                .HasColumnType("xml");

            configuration.Property(e => e.LabTestId)
                .IsRequired()
                .HasColumnName("LabTest_Id");

            configuration.Property(e => e.LabTestSource)
                .HasMaxLength(200);

            configuration.Property(e => e.LabValue)
                .HasMaxLength(20);

            configuration.Property(e => e.PatientLabTestGuid)
                .IsRequired()
                .HasDefaultValueSql("(newid())");

            configuration.Property(e => e.ReferenceLower)
                .HasMaxLength(20);

            configuration.Property(e => e.ReferenceUpper)
                .HasMaxLength(20);

            configuration.Property(e => e.TestDate)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.TestResult)
                .HasMaxLength(50);

            configuration.Property(e => e.TestUnitId)
                .HasColumnName("TestUnit_Id");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.AuditUserId)
                .HasConstraintName("FK_dbo.PatientLabTest_dbo.User_AuditUser_Id");

            configuration.HasOne(d => d.LabTest)
                .WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.LabTestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.PatientLabTest_dbo.LabTest_LabTest_Id");

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.PatientLabTest_dbo.Patient_Patient_Id");

            configuration.HasOne(d => d.TestUnit)
                .WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.TestUnitId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.PatientLabTest_dbo.LabTestUnit_TestUnit_Id");

            configuration.HasIndex(new string[] { "Patient_Id", "LabTest_Id" }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId, "IX_AuditUser_Id");
            configuration.HasIndex(e => e.LabTestId, "IX_LabTest_Id");
            configuration.HasIndex(e => e.PatientId, "IX_Patient_Id");
            configuration.HasIndex(e => e.TestUnitId, "IX_TestUnit_Id");
        }
    }
}
