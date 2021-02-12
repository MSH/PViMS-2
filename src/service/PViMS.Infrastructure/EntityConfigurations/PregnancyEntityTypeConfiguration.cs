using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PregnancyEntityTypeConfiguration : IEntityTypeConfiguration<Pregnancy>
    {
        public void Configure(EntityTypeBuilder<Pregnancy> configuration)
        {
            configuration.ToTable("Pregnancy");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ActualDeliveryDate)
                .HasColumnType("datetime");

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.ExpectedDeliveryDate)
                .HasColumnType("datetime");

            configuration.Property(e => e.FinishDate)
                .HasColumnType("datetime");

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.PreferredFeedingChoice)
                .IsRequired()
                .HasMaxLength(10);

            configuration.Property(e => e.StartDate)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.PregnancyCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.Pregnancy_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.Pregnancies)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.Pregnancy_dbo.Patient_Patient_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.PregnancyUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.Pregnancy_dbo.User_UpdatedBy_Id");

            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.PatientId, "IX_Patient_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
