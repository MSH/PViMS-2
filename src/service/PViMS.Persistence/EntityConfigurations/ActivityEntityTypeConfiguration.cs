using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ActivityEntityTypeConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> configuration)
        {
            configuration.ToTable("Activity");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.QualifiedName)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property<int>("ActivityType")
                .IsRequired();

            configuration.HasOne(c => c.WorkFlow)
                .WithMany()
                .HasForeignKey("WorkFlow_Id")
                .IsRequired(true);

            configuration.HasMany(c => c.ExecutionStatuses)
               .WithOne()
               .HasForeignKey("Activity_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("QualifiedName", "WorkFlow_Id").IsUnique(true);
        }
    }
}
