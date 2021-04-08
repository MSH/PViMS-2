using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ActivityInstanceEntityTypeConfiguration : IEntityTypeConfiguration<ActivityInstance>
    {
        public void Configure(EntityTypeBuilder<ActivityInstance> configuration)
        {
            configuration.ToTable("ActivityInstance");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.CurrentStatusId)
                .IsRequired()
                .HasColumnName("CurrentStatus_Id");

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(c => c.QualifiedName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.ReportInstanceId)
                .IsRequired()
                .HasColumnName("ReportInstance_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.Current)
                .IsRequired();

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.ActivityInstanceCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.ActivityInstance_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.CurrentStatus)
                .WithMany(p => p.ActivityInstances)
                .HasForeignKey(d => d.CurrentStatusId)
                .HasConstraintName("FK_dbo.ActivityInstance_dbo.ActivityExecutionStatus_CurrentStatus_Id");

            configuration.HasOne(d => d.ReportInstance)
                .WithMany(p => p.Activities)
                .HasForeignKey(d => d.ReportInstanceId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.ActivityInstance_dbo.ReportInstance_ReportInstance_Id1");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.ActivityInstanceUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.ActivityInstance_dbo.User_UpdatedBy_Id");

            configuration.HasIndex(e => new { e.QualifiedName, e.ReportInstanceId }).IsUnique(true);
            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.CurrentStatusId, "IX_CurrentStatus_Id");
            configuration.HasIndex(e => e.ReportInstanceId, "IX_ReportInstance_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
