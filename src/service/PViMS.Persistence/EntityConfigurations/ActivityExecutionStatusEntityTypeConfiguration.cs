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
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.HasOne(c => c.Activity)
                .WithMany()
                .HasForeignKey("Activity_Id")
                .IsRequired(true);

            configuration.Property(c => c.FriendlyDescription)
                .HasMaxLength(100)
                .IsRequired(false);

            configuration.HasIndex("Description", "Activity_Id").IsUnique(true);
        }
    }
}
