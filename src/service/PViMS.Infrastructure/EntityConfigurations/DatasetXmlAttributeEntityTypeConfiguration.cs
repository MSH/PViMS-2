using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetXmlAttributeEntityTypeConfiguration : IEntityTypeConfiguration<DatasetXmlAttribute>
    {
        public void Configure(EntityTypeBuilder<DatasetXmlAttribute> configuration)
        {
            configuration.ToTable("DatasetXmlAttribute");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.AttributeName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.DatasetElementId)
                .HasColumnName("DatasetElement_Id");

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(e => e.ParentNodeId)
                .IsRequired()
                .HasColumnName("ParentNode_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DatasetXmlAttributeCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.DatasetXmlAttribute_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.DatasetElement)
                .WithMany(p => p.DatasetXmlAttributes)
                .HasForeignKey(d => d.DatasetElementId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetXmlAttribute_dbo.DatasetElement_DatasetElement_Id");

            configuration.HasOne(d => d.ParentNode)
                .WithMany(p => p.NodeAttributes)
                .HasForeignKey(d => d.ParentNodeId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_dbo.DatasetXmlAttribute_dbo.DatasetXmlNode_ParentNode_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DatasetXmlAttributeUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.DatasetXmlAttribute_dbo.User_UpdatedBy_Id");

            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.DatasetElementId, "IX_DatasetElement_Id");
            configuration.HasIndex(e => e.ParentNodeId, "IX_ParentNode_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
