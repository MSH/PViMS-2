using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetXmlNodeEntityTypeConfiguration : IEntityTypeConfiguration<DatasetXmlNode>
    {
        public void Configure(EntityTypeBuilder<DatasetXmlNode> configuration)
        {
            configuration.ToTable("DatasetXmlNode");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Created)
                .IsRequired()
                .HasColumnType("datetime");

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.DatasetElementId)
                .HasColumnName("DatasetElement_Id");

            configuration.Property(e => e.DatasetElementSubId)
                .HasColumnName("DatasetElementSub_Id");

            configuration.Property(e => e.DatasetXmlId)
                .IsRequired()
                .HasColumnName("DatasetXml_Id");

            configuration.Property(e => e.LastUpdated)
                .HasColumnType("datetime");

            configuration.Property(c => c.NodeName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(c => c.NodeType)
                .HasConversion(x => (int)x, x => (NodeType)x);

            configuration.Property(e => e.ParentNodeId)
                .HasColumnName("ParentNode_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DatasetXmlNodeCreations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_dbo.DatasetXmlNode_dbo.User_CreatedBy_Id");

            configuration.HasOne(d => d.DatasetElement)
                .WithMany(p => p.DatasetXmlNodes)
                .HasForeignKey(d => d.DatasetElementId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetXmlNode_dbo.DatasetElement_DatasetElement_Id");

            configuration.HasOne(d => d.DatasetElementSub)
                .WithMany(p => p.DatasetXmlNodes)
                .HasForeignKey(d => d.DatasetElementSubId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetXmlNode_dbo.DatasetElementSub_DatasetElementSub_Id");

            configuration.HasOne(d => d.DatasetXml)
                .WithMany(p => p.ChildrenNodes)
                .HasForeignKey(d => d.DatasetXmlId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetXmlNode_dbo.DatasetXml_DatasetXml_Id");

            configuration.HasOne(d => d.ParentNode)
                .WithMany(p => p.ChildrenNodes)
                .HasForeignKey(d => d.ParentNodeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_dbo.DatasetXmlNode_dbo.DatasetXmlNode_ParentNode_Id");

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DatasetXmlNodeUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_dbo.DatasetXmlNode_dbo.User_UpdatedBy_Id");

            configuration.HasIndex(e => new { e.DatasetXmlId, e.NodeName }).IsUnique(true);
            configuration.HasIndex(e => e.CreatedById, "IX_CreatedBy_Id");
            configuration.HasIndex(e => e.DatasetElementSubId, "IX_DatasetElementSub_Id");
            configuration.HasIndex(e => e.DatasetElementId, "IX_DatasetElement_Id");
            configuration.HasIndex(e => e.DatasetXmlId, "IX_DatasetXml_Id");
            configuration.HasIndex(e => e.ParentNodeId, "IX_ParentNode_Id");
            configuration.HasIndex(e => e.UpdatedById, "IX_UpdatedBy_Id");
        }
    }
}
