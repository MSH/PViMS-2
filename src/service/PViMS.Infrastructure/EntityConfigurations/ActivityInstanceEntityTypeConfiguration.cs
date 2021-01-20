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

            configuration.Property(c => c.QualifiedName)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.Created)
                .IsRequired(true);

            configuration.Property(c => c.LastUpdated)
                .IsRequired(false);

            configuration.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedBy_Id");

            configuration.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey("UpdatedBy_Id");

            configuration.HasOne(c => c.CurrentStatus)
                .WithMany()
                .HasForeignKey("CurrentStatus_Id");

            configuration.HasOne(c => c.ReportInstance)
                .WithMany()
                .HasForeignKey("ReportInstance_Id");

            configuration.Property(c => c.Current)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.HasMany(c => c.ExecutionEvents)
               .WithOne()
               .HasForeignKey("ActivityInstance_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("QualifiedName", "ReportInstance_Id").IsUnique(true);
        }
    }
}
