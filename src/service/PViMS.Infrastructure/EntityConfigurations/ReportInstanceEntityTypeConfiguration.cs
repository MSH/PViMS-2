using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ReportInstanceEntityTypeConfiguration : IEntityTypeConfiguration<ReportInstance>
    {
        public void Configure(EntityTypeBuilder<ReportInstance> configuration)
        {
            configuration.ToTable("ReportInstance");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.Finished)
                .HasColumnType("datetime");

            configuration.Property(e => e.Identifier)
                .IsRequired();

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(e => e.PatientIdentifier)
                .IsRequired();

            configuration.Property(e => e.SourceIdentifier)
                .IsRequired();

            configuration.Property(e => e.TerminologyMedDraId)
                .HasColumnName("TerminologyMedDra_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(e => e.WorkFlowId)
                .IsRequired()
                .HasColumnName("WorkFlow_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.ReportInstanceCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.ReportInstance_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.TerminologyMedDra)
                .WithMany(p => p.ReportInstances)
                .HasForeignKey(d => d.TerminologyMedDraId)
                .HasConstraintName("FK_dbo.ReportInstance_dbo.TerminologyMedDra_TerminologyMedDra_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.ReportInstanceUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.ReportInstance_dbo.User_UpdatedBy_Id");

            configuration.HasOne(d => d.WorkFlow)
                .WithMany(p => p.ReportInstances)
                .HasForeignKey(d => d.WorkFlowId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ReportInstance_dbo.WorkFlow_WorkFlow_Id");

            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.TerminologyMedDraId, "IX_TerminologyMedDra_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
            configuration.HasIndex(e => e.WorkFlowId, "IX_WorkFlow_Id");
        }
    }
}
