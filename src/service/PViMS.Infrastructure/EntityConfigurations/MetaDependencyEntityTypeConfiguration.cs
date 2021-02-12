using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class MetaDependencyEntityTypeConfiguration : IEntityTypeConfiguration<MetaDependency>
    {
        public void Configure(EntityTypeBuilder<MetaDependency> configuration)
        {
            configuration.ToTable("MetaDependency");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.MetaDependencyGuid)
                .IsRequired()
                .HasColumnName("metadependency_guid");

            configuration.Property(e => e.ParentColumnName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.ParentTableId)
                .IsRequired()
                .HasColumnName("ParentTable_Id");

            configuration.Property(e => e.ReferenceColumnName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.ReferenceTableId)
                .IsRequired()
                .HasColumnName("ReferenceTable_Id");

            configuration.HasOne(d => d.ParentTable)
                .WithMany(p => p.MetaDependencyParentTables)
                .HasForeignKey(d => d.ParentTableId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.MetaDependency_dbo.MetaTable_ParentTable_Id");

            configuration.HasOne(d => d.ReferenceTable)
                .WithMany(p => p.MetaDependencyReferenceTables)
                .HasForeignKey(d => d.ReferenceTableId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.MetaDependency_dbo.MetaTable_ReferenceTable_Id");

            configuration.HasIndex(e => e.ParentTableId, "IX_ParentTable_Id");
            configuration.HasIndex(e => e.ReferenceTableId, "IX_ReferenceTable_Id");
        }
    }
}
