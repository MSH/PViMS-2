using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetXmlEntityTypeConfiguration : IEntityTypeConfiguration<DatasetXml>
    {
        public void Configure(EntityTypeBuilder<DatasetXml> configuration)
        {
            configuration.ToTable("DatasetXml");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DatasetXmlCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.DatasetXml_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DatasetXmlUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.DatasetXml_dbo.User_UpdatedBy_Id");

            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
