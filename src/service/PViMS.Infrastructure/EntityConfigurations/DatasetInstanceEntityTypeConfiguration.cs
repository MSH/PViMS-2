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

            configuration.Property(e => e.ContextId)
                .IsRequired()
                .HasColumnName("ContextID");

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.DatasetId)
                .IsRequired()
                .HasColumnName("Dataset_Id");

            configuration.Property(e => e.EncounterTypeWorkPlanId)
                .HasColumnName("EncounterTypeWorkPlan_Id");

            configuration.Property(c => c.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.DatasetInstanceGuid)
                .IsRequired()
                .HasDefaultValueSql("newid()");

            configuration.Property(c => c.Status)
                .HasConversion(x => (int)x, x => (DatasetInstanceStatus)x);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DatasetInstanceCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.DatasetInstance_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.Dataset)
                .WithMany(p => p.DatasetInstances)
                .HasForeignKey(d => d.DatasetId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetInstance_dbo.Dataset_Dataset_Id");

            configuration.HasOne(d => d.EncounterTypeWorkPlan)
                .WithMany(p => p.DatasetInstances)
                .HasForeignKey(d => d.EncounterTypeWorkPlanId)
                .HasConstraintName("FK_dbo.DatasetInstance_dbo.EncounterTypeWorkPlan_EncounterTypeWorkPlan_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DatasetInstanceUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.DatasetInstance_dbo.User_UpdatedBy_Id");

            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.DatasetId, "IX_Dataset_Id");
            configuration.HasIndex(e => e.EncounterTypeWorkPlanId, "IX_EncounterTypeWorkPlan_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
