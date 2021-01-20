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

            configuration.Property(c => c.EncounterDate)
                .IsRequired(true);

            configuration.Property(c => c.Notes)
                .IsRequired(false);

            configuration.Property(c => c.EncounterGuid)
                .IsRequired(true)
                .HasDefaultValueSql("newid()");

            configuration.Property(c => c.Discharged)
                .HasDefaultValue(false)
                .IsRequired(true);

            configuration.Property(c => c.CustomAttributesXmlSerialised)
                .IsRequired(false);

            configuration.Property(c => c.Created)
                .IsRequired(true);

            configuration.Property(c => c.LastUpdated)
                .IsRequired(false);

            configuration.HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedBy_Id");

            configuration.HasOne(c => c.EncounterType)
                .WithMany()
                .HasForeignKey("EncounterType_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.Patient)
                .WithMany()
                .HasForeignKey("Patient_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.Pregnancy)
                .WithMany()
                .HasForeignKey("Pregnancy_Id");

            configuration.HasOne(c => c.Priority)
                .WithMany()
                .HasForeignKey("Priority_Id");

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

            configuration.HasMany(c => c.Attachments)
               .WithOne()
               .HasForeignKey("Encounter_Id")
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("EncounterDate").IsUnique(false);
            configuration.HasIndex("Patient_Id", "EncounterDate").IsUnique(false);
        }
    }
}
