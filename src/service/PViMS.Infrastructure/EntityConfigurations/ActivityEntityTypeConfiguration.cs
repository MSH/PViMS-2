using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ActivityEntityTypeConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> configuration)
        {
            configuration.ToTable("Activity");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.QualifiedName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(c => c.ActivityType)
                .HasConversion(x => (int)x, x => (ActivityTypes)x);

            configuration.Property(e => e.WorkFlowId)
                .IsRequired()
                .HasColumnName("WorkFlow_Id");

            configuration.HasOne(c => c.WorkFlow)
                .WithMany(p => p.Activities)
                .HasForeignKey(c => c.WorkFlowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.Activity_dbo.WorkFlow_WorkFlow_Id");

            //configuration.HasMany(c => c.ExecutionStatuses)
            //   .WithOne(p => p.Activity)
            //   .HasForeignKey(c => c.ActivityId)
            //   .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex( new string[] { "QualifiedName", "WorkFlow_Id" }).IsUnique(true);
            configuration.HasIndex(e => e.WorkFlowId, "IX_WorkFlow_Id");
        }
    }
}
