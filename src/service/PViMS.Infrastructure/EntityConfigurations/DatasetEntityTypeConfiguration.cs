using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetEntityTypeConfiguration : IEntityTypeConfiguration<Dataset>
    {
        public void Configure(EntityTypeBuilder<Dataset> configuration)
        {
            configuration.ToTable("Dataset");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ContextTypeId)
                .IsRequired()
                .HasColumnName("ContextType_Id");

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(c => c.DatasetName)
                .IsRequired(true)
                .HasMaxLength(50);

            configuration.Property(e => e.DatasetXmlId)
                .HasColumnName("DatasetXml_Id");

            configuration.Property(e => e.EncounterTypeWorkPlanId)
                .HasColumnName("EncounterTypeWorkPlan_Id");

            configuration.Property(c => c.Help)
                .HasMaxLength(250);

            configuration.Property(c => c.InitialiseProcess)
                .HasMaxLength(100);

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(c => c.RulesProcess)
                .HasMaxLength(100);

            configuration.Property(c => c.Uid)
                .HasColumnName("UID")
                .HasMaxLength(10);

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.Active)
                .IsRequired()
                .HasDefaultValue(true);

            configuration.Property(c => c.IsSystem)
                .IsRequired()
                .HasDefaultValue(true);

            configuration.HasOne(d => d.ContextType)
                .WithMany(p => p.Datasets)
                .HasForeignKey(d => d.ContextTypeId)
                .HasConstraintName("FK_dbo.Dataset_dbo.ContextType_ContextType_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DatasetCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.Dataset_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.DatasetXml)
                .WithMany(p => p.Datasets)
                .HasForeignKey(d => d.DatasetXmlId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.Dataset_dbo.DatasetXml_DatasetXml_Id");

            configuration.HasOne(d => d.EncounterTypeWorkPlan)
                .WithMany(p => p.Datasets)
                .HasForeignKey(d => d.EncounterTypeWorkPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dbo.Dataset_dbo.EncounterTypeWorkPlan_EncounterTypeWorkPlan_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DatasetUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.Dataset_dbo.User_UpdatedBy_Id");

            configuration.HasIndex("DatasetName").IsUnique(true);
            configuration.HasIndex(e => e.ContextTypeId, "IX_ContextType_Id");
            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.DatasetXmlId, "IX_DatasetXml_Id");
            configuration.HasIndex(e => e.EncounterTypeWorkPlanId, "IX_EncounterTypeWorkPlan_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
