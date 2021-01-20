using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ActivityExecutionStatusEntityTypeConfiguration : IEntityTypeConfiguration<ActivityExecutionStatus>
    {
        public void Configure(EntityTypeBuilder<ActivityExecutionStatus> configuration)
        {
            configuration.ToTable("ActivityExecutionStatus");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.ActivityId)
                .HasColumnName("Activity_Id");

            configuration.Property(c => c.FriendlyDescription)
                .HasMaxLength(100);

            configuration.HasOne(c => c.Activity)
                .WithMany()
                .HasForeignKey("Activity_Id")
                .IsRequired(true);

            configuration.HasOne(d => d.Activity)
                .WithMany(p => p.ExecutionStatuses)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.ActivityExecutionStatus_dbo.Activity_Activity_Id");

            configuration.HasIndex(new string[] { "Description", "Activity_Id" }).IsUnique(true);
            configuration.HasIndex(e => e.ActivityId, "IX_Activity_Id");
        }
    }
}
}
}
