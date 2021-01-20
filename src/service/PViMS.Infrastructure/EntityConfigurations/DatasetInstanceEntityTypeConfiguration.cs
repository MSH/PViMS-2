using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetInstanceEntityTypeConfiguration : IEntityTypeConfiguration<DatasetInstance>
    {
        public void Configure(EntityTypeBuilder<DatasetInstance> configuration)
        {
            configuration.ToTable("DatasetInstance");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ContextID)
                .IsRequired(true);

            configuration.Property(c => c.Created)
                .IsRequired(true);

            configuration.Property(c => c.LastUpdated)
                .IsRequired(false);

            configuration.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedBy_Id");

            configuration.HasOne(c => c.Dataset)
                .WithMany()
                .HasForeignKey("Dataset_Id")
                .IsRequired(true);

            configuration.HasOne(c => c.EncounterTypeWorkPlan)
                .WithMany()
                .HasForeignKey("EncounterTypeWorkPlan_Id");

            configuration.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey("UpdatedBy_Id");

            configuration.Property(c => c.Tag)
                .IsRequired(false);

            configuration.Property(c => c.DatasetInstanceGuid)
                .IsRequired(true)
                .HasDefaultValueSql("newid()");

            configuration.Property(c => c.Status)
                .HasConversion(x => (int)x, x => (DatasetInstanceStatus)x);

            configuration.HasMany(c => c.DatasetInstanceValues)
               .WithOne()
               .HasForeignKey("DatasetInstance_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
