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

            configuration.Property(c => c.DatasetName)
                .HasMaxLength(50)
                .IsRequired(true);

            configuration.Property(c => c.Active)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.Property(c => c.InitialiseProcess)
                .HasMaxLength(100)
                .IsRequired(false);

            configuration.Property(c => c.RulesProcess)
                .HasMaxLength(100)
                .IsRequired(false);

            configuration.Property(c => c.Help)
                .HasMaxLength(250)
                .IsRequired(false);

            configuration.Property(c => c.Created)
                .IsRequired(true);

            configuration.Property(c => c.LastUpdated)
                .IsRequired(false);

            configuration.HasOne(c => c.ContextType)
                .WithMany()
                .HasForeignKey("ContextType_Id")
                .IsRequired(true);

            configuration.HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey("CreatedBy_Id");

            configuration.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey("UpdatedBy_Id");

            configuration.HasOne(c => c.EncounterTypeWorkPlan)
                .WithMany()
                .HasForeignKey("EncounterTypeWorkPlan_Id");

            configuration.Property(c => c.UID)
                .HasMaxLength(10)
                .IsRequired(false);

            configuration.Property(c => c.IsSystem)
                .HasDefaultValue(true)
                .IsRequired(true);

            configuration.HasOne(c => c.DatasetXml)
                .WithMany()
                .HasForeignKey("DatasetXml_Id");

            configuration.HasMany(c => c.DatasetCategories)
               .WithOne()
               .HasForeignKey("Dataset_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasMany(c => c.DatasetRules)
               .WithOne()
               .HasForeignKey("Dataset_Id")
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("DatasetName").IsUnique(true);
        }
    }
}
