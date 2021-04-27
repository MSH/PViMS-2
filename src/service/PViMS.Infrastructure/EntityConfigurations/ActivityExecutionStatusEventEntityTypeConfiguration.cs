using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ActivityExecutionStatusEventEntityTypeConfiguration : IEntityTypeConfiguration<ActivityExecutionStatusEvent>
    {
        public void Configure(EntityTypeBuilder<ActivityExecutionStatusEvent> configuration)
        {
            configuration.ToTable("ActivityExecutionStatusEvent");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ActivityInstanceId)
                .IsRequired()
                .HasColumnName("ActivityInstance_Id");

            configuration.Property(c => c.ContextCode)
                .HasMaxLength(20);

            configuration.Property(c => c.ContextDateTime)
                .HasColumnType("datetime");

            configuration.Property(e => e.EventCreatedById)
                .IsRequired()
                .HasColumnName("EventCreatedBy_Id");

            configuration.Property(c => c.EventDateTime)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.ExecutionStatusId)
                .IsRequired()
                .HasColumnName("ExecutionStatus_Id");

            configuration.HasOne(d => d.ActivityInstance)
                .WithMany(p => p.ExecutionEvents)
                .HasForeignKey(d => d.ActivityInstanceId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.ActivityExecutionStatusEvent_dbo.ActivityInstance_ActivityInstance_Id");

            configuration.HasOne(d => d.EventCreatedBy)
                .WithMany(p => p.ExecutionEvents)
                .HasForeignKey(d => d.EventCreatedById)
                .HasConstraintName("FK_dbo.ActivityExecutionStatusEvent_dbo.User_EventCreatedBy_Id");

            configuration.HasOne(d => d.ExecutionStatus)
                .WithMany(p => p.ExecutionEvents)
                .HasForeignKey(d => d.ExecutionStatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ActivityExecutionStatusEvent_dbo.ActivityExecutionStatus_ExecutionStatus_Id");

            configuration.HasIndex(e => new { e.EventDateTime, e.ActivityInstanceId, e.ExecutionStatusId }).IsUnique(true);
            configuration.HasIndex(e => e.ActivityInstanceId, "IX_ActivityInstance_Id");
            configuration.HasIndex(e => e.EventCreatedById, "IX_EventCreatedBy_Id");
            configuration.HasIndex(e => e.ExecutionStatusId, "IX_ExecutionStatus_Id");
        }
    }
}
