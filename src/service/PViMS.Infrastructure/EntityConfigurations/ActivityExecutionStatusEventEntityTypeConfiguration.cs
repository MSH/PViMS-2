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

            configuration.Property(c => c.EventDateTime)
                .IsRequired(true);

            configuration.Property(c => c.Comments)
                .IsRequired(false);

            configuration.HasOne(c => c.ActivityInstance)
                .WithMany()
                .HasForeignKey("ActivityInstance_Id")
                .IsRequired(true);

            configuration.HasOne(p => p.EventCreatedBy)
                .WithMany()
                .HasForeignKey("EventCreatedBy_Id");

            configuration.HasOne(p => p.ExecutionStatus)
                .WithMany()
                .HasForeignKey("ExecutionStatus_Id")
                .IsRequired(true);

            configuration.Property(c => c.ContextDateTime)
                .IsRequired(false);

            configuration.Property(c => c.ContextCode)
                .HasMaxLength(20)
                .IsRequired(false);

            configuration.HasMany(c => c.Attachments)
               .WithOne()
               .HasForeignKey("ActivityExecutionStatusEvent_Id")
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("ActivityInstance_Id", "ExecutionStatus_Id").IsUnique(true);
        }
    }
}
